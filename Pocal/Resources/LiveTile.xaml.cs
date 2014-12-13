using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using Pocal.ViewModel;
using Windows.ApplicationModel.Appointments;
using System.Threading.Tasks;

namespace Pocal.Resources
{
    public partial class LiveTile : UserControl
    {
        public LiveTile()
        {
            InitializeComponent();

        }

        public static async Task<Appointment> getNextAppointment()
        {
            IReadOnlyList<Appointment> appts = await CalendarAPI.getAppointments(DateTime.Now, 2);

            foreach (Appointment appt in appts)
            {
               if (appt.StartTime >= DateTime.Now)
               {
                   return appt;

               }
            }

            ////foreach (Day day in App.ViewModel.Days)
            ////{
            ////   foreach (PocalAppointment pa in day.PocalApptsOfDay)
            ////    {
            ////        if (pa.StartTime < DateTime.Now)
            ////            continue;
            ////        else
            ////            return pa;

            ////    } 
            ////}
            return null;
        }

        public void UpdateTextBox(Appointment appt)
        {

            dayOfWeekTb.Text = DateTime.Now.DayOfWeek.ToString().Substring(0, 2);
            dayTb.Text = DateTime.Now.Day.ToString();
            if (appt == null)
            {
                tb1.Text = "";
                tb2.Text = "";
                LayoutRoot.UpdateLayout();
            }
            else
            {
                tb1.Text = appt.Subject;
               
                tb2.Text = tb2Text(appt);
                LayoutRoot.UpdateLayout();
            }
           
        }

        private static string tb2Text(Appointment appt)
        {
            DateTimeOffset endTime = appt.StartTime + appt.Duration;
            if (appt.StartTime.Date != endTime.Date)
            {
                return (appt.StartTime.DayOfWeek.ToString() + " bis " +endTime.DayOfWeek.ToString());
            }
            return (appt.StartTime.Hour.ToString("00") + ":" + appt.StartTime.Minute.ToString("00") + " - " + endTime.Hour.ToString("00") + ":" + endTime.Minute.ToString("00"));
        }



        //public async void DoUpdateTile()
        //{
        //    await  setNextAppointment();
        //    UpdateTile();
        //}


        public static async void UpdateTile()
        {
            Appointment appt = await getNextAppointment();

            var customTile = new LiveTile();
            customTile.Measure(new Size(336, 336));
            customTile.Arrange(new Rect(0, 0, 336, 336));
            customTile.UpdateTextBox(appt);

           


            var customTileWide = new LiveTile();
            customTileWide.Measure(new Size(691, 336));
            customTileWide.Arrange(new Rect(0, 0, 691, 336));
            customTileWide.UpdateTextBox(appt);

            var bmp = new WriteableBitmap(336, 336);
            bmp.Render(customTile, null);
            bmp.Invalidate();

            var bmp2 = new WriteableBitmap(691, 336);
            bmp2.Render(customTileWide, null);
            bmp2.Invalidate();

            const string filename = "/Shared/ShellContent/CustomTile.jpg";
            const string filenameWide = "/Shared/ShellContent/CustomTileWide.jpg";

            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isf.DirectoryExists("/CustomLiveTiles"))
                {
                    isf.CreateDirectory("/CustomLiveTiles");
                }

                using (var stream = isf.OpenFile(filename, System.IO.FileMode.OpenOrCreate))
                {
                    bmp.SaveJpeg(stream, 336, 336, 0, 100);
                }

                using (var stream = isf.OpenFile(filenameWide, System.IO.FileMode.OpenOrCreate))
                {
                    bmp2.SaveJpeg(stream, 691, 336, 0, 100);
                }
            }
            try
            {

                FlipTileData tileData = new FlipTileData
                {
                    //Title = "CustomSecondaryTile", 
                    WideBackgroundImage = new Uri("isostore:" + filenameWide, UriKind.Absolute),
                    BackgroundImage = new Uri("isostore:" + filename, UriKind.Absolute),
                };

                //string tileUri = string.Concat("/MainPage.xaml?", "");
                ShellTile currentTile = ShellTile.ActiveTiles.FirstOrDefault();
                currentTile.Update(tileData);
                //ShellTile.Create(new Uri(tileUri, UriKind.Relative), tileData, true);
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
        }
    }
}
