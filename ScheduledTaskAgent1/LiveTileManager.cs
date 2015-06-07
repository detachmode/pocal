using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel.Appointments;
using Cimbalino.Phone.Toolkit.Extensions;
using Microsoft.Phone.Shell;
using Shared.Helper;


namespace ScheduledTaskAgent1
{
    public static class LiveTileManager
    {
        public static List<Appointment> AppointmentsOnLiveTile;

        public static async Task<List<Appointment>> GetNextAppointments()
        {
            var howManyDays = 2;
            if (IsolatedStorageSettings.ApplicationSettings.Contains("TileDaysInFuture"))
            {
                howManyDays = (int)IsolatedStorageSettings.ApplicationSettings["TileDaysInFuture"];
            }

            var nextAppointments = new List<Appointment>();
            var appts = (await CalendarAPI.GetAppointments(DateTime.Now.Date, howManyDays)).ToList<Appointment>();

            // Options: Sortiere Termine aus, die von hidden Kalender sind.
            if (IsolatedStorageSettings.ApplicationSettings.Contains("HiddenCalendars"))
            {
                var hiddenCalendars = (List<string>)IsolatedStorageSettings.ApplicationSettings["HiddenCalendars"];
                foreach (var id in hiddenCalendars)
                {
                    appts.RemoveAll(x => x.CalendarId == id);
                }
            }


            // der nächste Termin, der nicht AllDay ist:
            foreach (var appt in appts)
            {
                if (appt.AllDay)
                    continue;

                if (appt.StartTime >= DateTime.Now)
                {
                    nextAppointments.Add(appt);
                    break;
                }
            }


            foreach (var appt in appts)
            {
                if (nextAppointments.Count == 0 || appt != nextAppointments[0])
                {
                    if (appt.StartTime >= DateTime.Now)
                    {
                        nextAppointments.Add(appt);
                    }
                    else if (appt.AllDay)
                        nextAppointments.Add(appt);
                }
            }

            return nextAppointments;
        }

        public static bool IsSingleLiveTileEnabled()
        {
            var isSingleLiveTileEnabled = false;
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LiveTileSettingsSingle"))
            {
                isSingleLiveTileEnabled = (bool)IsolatedStorageSettings.ApplicationSettings["LiveTileSettingsSingle"];
            }
            return isSingleLiveTileEnabled;
        }

        public static string GetLocationStringWide(Appointment appt)
        {
            var str = "";
            if (appt.Location != "")
                str = appt.Location;

            return str;
        }

        public static string GetTimeStringWide(Appointment appt)
        {
            var endTime = appt.StartTime + appt.Duration;
            var str = "";

            if (appt.StartTime.Date >= DateTime.Now.Date.AddDays(2))
            {
                //  Weekname kürzel Date  Beispiel: Dienstag 04.05 
                str += appt.StartTime.Date.ToString("dddd", CultureInfo.CurrentUICulture);
                str += ": ";
                str += appt.StartTime.Date.ToShortDateString();

                return str;
            }

            str = StrTomorrow(appt, str);

            if (appt.AllDay)
            {
                str = StrAllday(str);
            }

            else if (appt.StartTime.Date != endTime.Date)
            {
                // Wochentag - Wochentag
                str += appt.StartTime.ToString("dddd", CultureInfo.CurrentUICulture);
                str += " - ";
                str += endTime.ToString("dddd", CultureInfo.CurrentUICulture);
            }

            else
            {
                // Uhrzeit - Uhrzeit
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

        private static string StrAllday(string str)
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
            return str;
        }

        private static string StrTomorrow(Appointment appt, string str)
        {
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
            return str;
        }

        public static string GetTimeStringNormal(Appointment appt)
        {
            var endTime = appt.StartTime + appt.Duration;
            var str = "";

            str = StrTomorrow(appt, str);

            if (appt.AllDay)
            {
                // Ganztägig
                str = StrAllday(str);
            }
            else if (appt.StartTime.Date != endTime.Date)
            {
                // Wochentag - Wochentag
                str += appt.StartTime.ToString("dddd", CultureInfo.CurrentUICulture);
                str += " - ";
                str += endTime.ToString("dddd", CultureInfo.CurrentUICulture);
            }
            else
            {
                // Start Uhrzeit 
                if (CultureInfo.CurrentUICulture.Name.Contains("en-"))
                {
                    str += string.Format("{0:h:mm tt}", appt.StartTime); // AM PM
                }
                else
                {
                    str += string.Format("{0:H:mm}", appt.StartTime); // 24:00
                }
            }
            return str;
        }

        public static void UpdateTile()
        {
            var customTile = new LiveTile();
            customTile.UpdateTextBox(AppointmentsOnLiveTile);
            customTile.Measure(new Size(336, 336));
            customTile.Arrange(new Rect(0, 0, 336, 336));


            var customTileWide = new LiveTileWide();
            customTileWide.UpdateTextBox(AppointmentsOnLiveTile);
            customTileWide.Measure(new Size(691, 336));
            customTileWide.Arrange(new Rect(0, 0, 691, 336));

            var customTileSmall = new LiveTileSmall();
            customTileSmall.UpdateDate();
            customTileSmall.Measure(new Size(159, 159));
            customTileSmall.Arrange(new Rect(0, 0, 159, 159));


            var bmp = new WriteableBitmap(336, 336);
            bmp.Render(customTile, null);
            bmp.Invalidate();

            var bmp2 = new WriteableBitmap(691, 336);
            bmp2.Render(customTileWide, null);
            bmp2.Invalidate();

            var bmp3 = new WriteableBitmap(159, 159);
            bmp3.Render(customTileSmall, null);
            bmp3.Invalidate();

            const string filename = "CustomTile";
            const string filenamefull = "/Shared/ShellContent/CustomTile.png";
            const string filenameWide = "CustomTileWide";
            const string filenameWidefull = "/Shared/ShellContent/CustomTileWide.png";
            const string filenameSmall = "CustomTileSmall";
            const string filenameSmallfull = "/Shared/ShellContent/CustomTileSmall.png";

            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (
                    var imageStream = new IsolatedStorageFileStream("/Shared/ShellContent/" + filename + ".png",
                        FileMode.OpenOrCreate, isf))
                {
                    WriteableBitmapExtensions.SavePng(bmp, imageStream);
                }

                using (
                    var imageStream = new IsolatedStorageFileStream("/Shared/ShellContent/" + filenameWide + ".png",
                        FileMode.OpenOrCreate, isf))
                {
                    WriteableBitmapExtensions.SavePng(bmp2, imageStream);
                }

                using (
                     var imageStream = new IsolatedStorageFileStream("/Shared/ShellContent/" + filenameSmall + ".png",
                        FileMode.OpenOrCreate, isf))
                {
                    WriteableBitmapExtensions.SavePng(bmp3, imageStream);
                }



            }



            var tileData = new FlipTileData
            {
                WideBackgroundImage = new Uri("isostore:" + filenameWidefull, UriKind.Absolute),
                BackgroundImage = new Uri("isostore:" + filenamefull, UriKind.Absolute),
                SmallBackgroundImage = new Uri("isostore:" + filenameSmallfull, UriKind.Absolute)
            };


            var currentTile = ShellTile.ActiveTiles.FirstOrDefault();
            if (currentTile != null)
            {
                currentTile.Update(tileData);
            }
        }

        public static async void UpdateTileFromForeground()
        {
            try
            {
                AppointmentsOnLiveTile = await GetNextAppointments();
                UpdateTile();
            }
            catch
            {
                // ignored
            }
        }
    }
}