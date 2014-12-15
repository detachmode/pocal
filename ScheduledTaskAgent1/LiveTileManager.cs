using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel.Appointments;

namespace ScheduledTaskAgent1
{
    public static class LiveTileManager
    {
        public static Appointment AppointmentOnLiveTile;
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
            return null;
        }


        public static string tb2TextWide(Appointment appt)
        {
            DateTimeOffset endTime = appt.StartTime + appt.Duration;
            string str = "";

            if (appt.StartTime.DayOfYear != DateTime.Now.DayOfYear)
            {
                str += "Morgen: ";
            }
            if (appt.StartTime.Date != endTime.Date)
            {
                str += (appt.StartTime.DayOfWeek.ToString() + " bis " + endTime.DayOfWeek.ToString());
            }

            str += (appt.StartTime.Hour.ToString("00") + ":" + appt.StartTime.Minute.ToString("00") + " - " + endTime.Hour.ToString("00") + ":" + endTime.Minute.ToString("00"));
            return str;

        }


        public static string tb2TextNormal(Appointment appt)
        {
            DateTimeOffset endTime = appt.StartTime + appt.Duration;
            string str = "";

            if (appt.StartTime.DayOfYear != DateTime.Now.DayOfYear)
            {
                str += "Morgen: ";
            }
            if (appt.StartTime.Date != endTime.Date)
            {
                str += (appt.StartTime.DayOfWeek.ToString() + " bis " + endTime.DayOfWeek.ToString());
            }

            str += (appt.StartTime.Hour.ToString("00") + ":" + appt.StartTime.Minute.ToString("00"));
            return str;
        }


        public static async void UpdateTile()
        {

            var customTile = new LiveTile();
            customTile.UpdateTextBox(AppointmentOnLiveTile);
            customTile.Measure(new Size(336, 336));
            customTile.Arrange(new Rect(0, 0, 336, 336));



            var customTileWide = new LiveTileWide();
            customTileWide.UpdateTextBox(AppointmentOnLiveTile);
            customTileWide.Measure(new Size(691, 336));
            customTileWide.Arrange(new Rect(0, 0, 691, 336));
            

            var bmp = new WriteableBitmap(336, 336);
            bmp.Render(customTile, null);
            bmp.Invalidate();

            var bmp2 = new WriteableBitmap(691, 336);
            bmp2.Render(customTileWide, null);
            bmp2.Invalidate();

            const string filename = "CustomTile";
            const string filenamefull = "/Shared/ShellContent/CustomTile.png";
            const string filenameWide = "CustomTileWide";
            const string filenameWidefull = "/Shared/ShellContent/CustomTileWide.png";

            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                //if (!isf.DirectoryExists("/CustomLiveTiles"))
                //{
                //    isf.CreateDirectory("/CustomLiveTiles");
                //}


                using (IsolatedStorageFileStream imageStream = new IsolatedStorageFileStream("/Shared/ShellContent/" + filename + ".png", System.IO.FileMode.OpenOrCreate, isf))
                {
                    Cimbalino.Phone.Toolkit.Extensions.WriteableBitmapExtensions.SavePng(bmp, imageStream);

                }

                using (IsolatedStorageFileStream imageStream = new IsolatedStorageFileStream("/Shared/ShellContent/" + filenameWide + ".png", System.IO.FileMode.OpenOrCreate, isf))
                {
                    Cimbalino.Phone.Toolkit.Extensions.WriteableBitmapExtensions.SavePng(bmp2, imageStream);

                }


                //using (var stream = isf.OpenFile(filename, System.IO.FileMode.OpenOrCreate))
                //{
                //    bmp.SaveJpeg(stream, 336, 336, 0, 100);
                //}

                //using (var stream = isf.OpenFile(filenameWide, System.IO.FileMode.OpenOrCreate))
                //{
                //    bmp2.SaveJpeg(stream, 691, 336, 0, 100);
                //}
            }
            try
            {

                FlipTileData tileData = new FlipTileData
                {
                    //Title = "CustomSecondaryTile", 
                    WideBackgroundImage = new Uri("isostore:" + filenameWidefull, UriKind.Absolute),
                    BackgroundImage = new Uri("isostore:" + filenamefull, UriKind.Absolute),
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
