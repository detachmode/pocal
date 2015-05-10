using Microsoft.Phone.Shell;
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
            List<Appointment> appts = (await CalendarAPI.GetAppointments(DateTime.Now.Date, 2)).ToList<Appointment>();

            // Options: Sortiere Termine aus, die von hidden Kalender sind.
            if (IsolatedStorageSettings.ApplicationSettings.Contains("HiddenCalendars"))
            {
                List<string> hiddenCalendars = (List<string>)IsolatedStorageSettings.ApplicationSettings["HiddenCalendars"];
                foreach (string id in hiddenCalendars)
                {
                    appts.RemoveAll(x => x.CalendarId == id);
                }
               
            }


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
                    else if (appt.AllDay)
                        nextAppointments.Add(appt);

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

        public static string getLocationStringWide(Appointment appt)
        {
            string str = "";
            if (appt.Location != "")
                str = appt.Location;

            return str;

        }

        public static string getTimeStringWide(Appointment appt)
        {
            DateTimeOffset endTime = appt.StartTime + appt.Duration;
            string str = "";

            str = strTomorrow(appt, str);

            if (appt.AllDay)
            {
                str = strAllday(str);

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

        private static string strAllday(string str)
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

        private static string strTomorrow(Appointment appt, string str)
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


        public static string getTimeStringNormal(Appointment appt)
        {
            DateTimeOffset endTime = appt.StartTime + appt.Duration;
            string str = "";

            str = strTomorrow(appt, str);

            if (appt.AllDay)
            {
                // Ganztägig
                str = strAllday(str);
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
                    str += string.Format("{0:h:mm tt}", appt.StartTime);  // AM PM
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

                using (IsolatedStorageFileStream imageStream = new IsolatedStorageFileStream("/Shared/ShellContent/" + filename + ".png", System.IO.FileMode.OpenOrCreate, isf))
                {
                    Cimbalino.Phone.Toolkit.Extensions.WriteableBitmapExtensions.SavePng(bmp, imageStream);

                }

                using (IsolatedStorageFileStream imageStream = new IsolatedStorageFileStream("/Shared/ShellContent/" + filenameWide + ".png", System.IO.FileMode.OpenOrCreate, isf))
                {
                    Cimbalino.Phone.Toolkit.Extensions.WriteableBitmapExtensions.SavePng(bmp2, imageStream);

                }

            }


            FlipTileData tileData = new FlipTileData
                {
                    WideBackgroundImage = new Uri("isostore:" + filenameWidefull, UriKind.Absolute),
                    BackgroundImage = new Uri("isostore:" + filenamefull, UriKind.Absolute),
                };


            ShellTile currentTile = ShellTile.ActiveTiles.FirstOrDefault();
            if (currentTile != null)
            {
                currentTile.Update(tileData);
            }
            

        }

        public async static void UpdateTileFromForeground()
        {
            try
            {
                AppointmentsOnLiveTile = await getNextAppointments();
                UpdateTile();
            }
            catch { }

        }
    }
}
