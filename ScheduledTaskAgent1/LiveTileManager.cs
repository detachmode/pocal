﻿using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel.Appointments;
using Shared.Helper;

namespace ScheduledTaskAgent1
{
    public static class LiveTileManager
    {
        public static List<Appointment> AppointmentsOnLiveTile;
        public static async Task<List<Appointment>> getNextAppointments()
        {
            List<Appointment> nextAppointments = new List<Appointment>();
            IReadOnlyList<Appointment> appts = await CalendarAPI.GetAppointments(DateTime.Now, 2);

            // der nächste Termin, der nicht AllDay ist:
            foreach (Appointment appt in appts)
            {
                if (appt.AllDay)
                    continue;

                if (appt.StartTime >= DateTime.Now)
                {
                    nextAppointments.Add(appt);
                    break;
                }

            }


            foreach (Appointment appt in appts)
            {
                if (nextAppointments.Count == 0 || appt != nextAppointments[0])
                {
                    if (appt.StartTime >= DateTime.Now)
                    {
                        nextAppointments.Add(appt);  
                    }
                    
                }
            }

            return nextAppointments;
        }

        public static bool IsSingleLiveTileEnabled()
        {
            bool isSingleLiveTileEnabled = false;
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LiveTileSettingsSingle"))
            {
                isSingleLiveTileEnabled = (bool)IsolatedStorageSettings.ApplicationSettings["LiveTileSettingsSingle"];
            }
            return isSingleLiveTileEnabled;
        }

        public static string tbOrtWide(Appointment appt)
        {
            string str = "";
            if (appt.Location != "")
                str = appt.Location;

            return str;

        }

        public static string tb2TextWide(Appointment appt)
        {
            DateTimeOffset endTime = appt.StartTime + appt.Duration;
            string str = "";

            if (appt.StartTime.DayOfYear != DateTime.Now.DayOfYear)
            {
                if (CultureInfo.CurrentUICulture.Name.Contains("de-"))
                {
                    str += "Morgen: ";
                }
                else if (CultureInfo.CurrentUICulture.Name.Contains("it-"))
                {
                    str += "Domani: ";
                }
                else
                    str += "Tomorrow: ";
            }
            if (appt.AllDay)
            {
                if (CultureInfo.CurrentUICulture.Name.Contains("de-"))
                {
                    str += "Ganztägig";
                }
                else if (CultureInfo.CurrentUICulture.Name.Contains("it-"))
                {
                    str += "Giornata intera";
                }
                else
                    str += "All day";

            }
            else if (appt.StartTime.Date != endTime.Date)
            {
                str += appt.StartTime.ToString("dddd", CultureInfo.CurrentUICulture);
                str += " - ";
                str += endTime.ToString("dddd", CultureInfo.CurrentUICulture);
            }
            else
            {
                if (CultureInfo.CurrentUICulture.Name.Contains("en-"))
                {
                    str += string.Format("{0:h:mm tt}", appt.StartTime);
                    str += " - ";
                    str += string.Format("{0:h:mm tt}", endTime);
                }
                else
                {
                    str += string.Format("{0:H:mm}", appt.StartTime);
                    str += " - ";
                    str += string.Format("{0:H:mm}", endTime);
                }
            }
            return str;

        }


        public static string tb2TextNormal(Appointment appt)
        {
            DateTimeOffset endTime = appt.StartTime + appt.Duration;
            string str = "";

            if (appt.StartTime.DayOfYear != DateTime.Now.DayOfYear)
            {
                if (CultureInfo.CurrentUICulture.Name.Contains("de-"))
                {
                    str += "Morgen: ";
                }
                else
                    str += "Tomorrow: ";
            }
            if (appt.AllDay)
            {
                if (CultureInfo.CurrentUICulture.Name.Contains("de-"))
                {
                    str += "Ganztägig";
                }
                else
                    str += "All day";

            }
            else if (appt.StartTime.Date != endTime.Date)
            {
                str += appt.StartTime.ToString("dddd", CultureInfo.CurrentUICulture);
                str += " - ";
                str += endTime.ToString("dddd", CultureInfo.CurrentUICulture);
            }
            else
            {
                if (CultureInfo.CurrentUICulture.Name.Contains("de-"))
                {
                    str += string.Format("{0:H:mm}", appt.StartTime);
                }
                else
                {
                    str += string.Format("{0:h:mm tt}", appt.StartTime);
                }
            }
            return str;
        }




        public static async void UpdateTile()
        {
            var customTile = new LiveTile();
            customTile.UpdateTextBox(AppointmentsOnLiveTile);
            customTile.Measure(new Size(336, 336));
            customTile.Arrange(new Rect(0, 0, 336, 336));



            var customTileWide = new LiveTileWide();
            customTileWide.UpdateTextBox(AppointmentsOnLiveTile);
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
                try
                {
                    using (IsolatedStorageFileStream imageStream = new IsolatedStorageFileStream("/Shared/ShellContent/" + filename + ".png", System.IO.FileMode.OpenOrCreate, isf))
                    {
                        Cimbalino.Phone.Toolkit.Extensions.WriteableBitmapExtensions.SavePng(bmp, imageStream);

                    }

                    using (IsolatedStorageFileStream imageStream = new IsolatedStorageFileStream("/Shared/ShellContent/" + filenameWide + ".png", System.IO.FileMode.OpenOrCreate, isf))
                    {
                        Cimbalino.Phone.Toolkit.Extensions.WriteableBitmapExtensions.SavePng(bmp2, imageStream);

                    }
                }
                catch { }

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
            catch
            {
                //MessageBox.Show(e.Message);
            }
        }

        public async static void UpdateTileFromForeground()
        {
            AppointmentsOnLiveTile = await getNextAppointments();
            UpdateTile();

        }
    }
}
