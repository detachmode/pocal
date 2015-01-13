using Pocal.ViewModel;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Windows.ApplicationModel.Appointments;
using System.Linq;
using System.Windows;
using Pocal.Helper;
using Pocal.Resources;

namespace Pocal.Converter
{
    public static class converterBrushes
    {
        public static SolidColorBrush pastDays = new SolidColorBrush(Color.FromArgb(255, 170, 170, 170));
        public static SolidColorBrush noWeekendHeader = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        public static SolidColorBrush DarkGray = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
        public static SolidColorBrush Black = new SolidColorBrush(Colors.Black);
        public static SolidColorBrush Red = new SolidColorBrush(Colors.Red);

    }


    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {

                DateTime dt = (DateTime)value;
                return dt.ToString("dddd", CultureSettings.ci) + ", " + dt.ToString("M", CultureSettings.ci);

            }
            else return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }
    }


    public class DeltaTimeFirstLine : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Day)
            {
                DateTime dt = ((Day)value).DT;
                return DeltaTimeStringCreator.getDeltaTime(dt);

            }
            else return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }


    }

    public class DeltaTimeSecondLineVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                TimeSpan ts = ((DateTime)value).Date - DateTime.Now.Date;

                if (ts.Days % 7 == 0 || Math.Abs(ts.Days) < 7)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }


    }


    public class hourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var appt = (value as PocalAppointment).Appt;
            if (appt != null)
            {
                if (appt.AllDay == true)
                {
                    return "";
                }
                else
                    return appt.StartTime.Hour;


            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }


    }



    public class minuteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var appt = (value as PocalAppointment).Appt;
            if (appt != null)
            {
                if (appt.AllDay == true)
                {
                    return "";
                }
                else
                {
                    int i = appt.StartTime.Minute;
                    return i.ToString("00");

                }


            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }


    }


    public class startTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var appt = (value as PocalAppointment);

            if (appt != null)
            {
                string str = "";

                if (appt.AllDay || appt.Duration == TimeSpan.FromDays(1))
                {
                    if (!AppResources.ResourceLanguage.Contains("en"))
                        return str;
                    else
                        return AppResources.allDay;
                }

                if (AppResources.ResourceLanguage.Contains("en"))
                    str = convert12(appt);
                else
                    str = convert24(appt);

                return str;
            }
            return "";
        }

        private string convert12(PocalAppointment appt)
        {
            return string.Format("{0:h:mm tt}", appt.StartTime);
        }

        private static string convert24(PocalAppointment appt)
        {
            return string.Format("{0:HH:mm}", appt.StartTime);
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }



    }


    public class secondLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var appt = (value as Appointment);

            if (appt != null)
            {
                string str = "";


                //Zeit
                if (appt.AllDay || appt.Duration >= TimeSpan.FromDays(1))
                {
                    int d = ((appt.StartTime + appt.Duration).Date - appt.StartTime.Date).Days;
                    if (d == 1)
                    {
                        str += d + " "+AppResources.DaySingular;
                    }
                    else
                    {
                        str += d + " " + AppResources.DayPlural;
                    }

                }
                else
                {
                    if (appt.Duration.Hours != 0)
                    {
                        
                        if (appt.Duration.Hours > 1)
                            str = appt.Duration.Hours + " " + AppResources.HourPlural;
                        else
                            str = appt.Duration.Hours + " " + AppResources.HourSingular;
                        str += " ";
                    }

                    if (appt.Duration.Minutes != 0)
                    {
                       
                        if (appt.Duration.Minutes > 1)
                            str += "" + appt.Duration.Minutes + " " + AppResources.MinutePlural;
                        else
                            str += "" + appt.Duration.Minutes + " " + AppResources.MinuteSingular;

                    }


                }

                // Location
                if (String.IsNullOrWhiteSpace(appt.Location))
                {
                    // Notes
                    if (!String.IsNullOrWhiteSpace(appt.Details))
                    {
                        string line1 = appt.Details.Split(new[] { '\r', '\n' }).FirstOrDefault();
                        str += " (" + line1 + ")";
                    }
                }
                else
                    str += " (" + appt.Location + ")";

                return str;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }



    }





    public class pastDaysBackgroundInverted : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt = (DateTime)value;
            //if (dt.Date < DateTime.Now.Date)
            int weeknumber = (dt.DayOfYear + 1) / 7;
            if (weeknumber % 2 == 1)
                return converterBrushes.DarkGray;
            else
                return converterBrushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class pastDaysBackground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt = (DateTime)value;
            //if (dt.Date < DateTime.Now.Date)
            int weeknumber = (dt.DayOfYear + 1) / 7;
            if (weeknumber % 2 == 0)
                return converterBrushes.DarkGray;
            else
                return converterBrushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class secondLinesForeground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt = (DateTime)value;

            if (dt.Date < DateTime.Now.Date)
                return converterBrushes.pastDays;

            return (App.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class dayHeadersForeground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt = (DateTime)value;

            if (dt.Date < DateTime.Now.Date)
                return converterBrushes.pastDays;

            //if (dt.Date == DateTime.Now.Date)
            //    return converterBrushes.Red;



            return (App.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class weekendForeground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt = (DateTime)value;

            if ((dt.DayOfWeek == DayOfWeek.Saturday) || (dt.DayOfWeek == DayOfWeek.Sunday))
            {
                return converterBrushes.pastDays;
            }
            else
                return converterBrushes.noWeekendHeader;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class weekNumberVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var day = value as Day;
            if (day.DT.DayOfWeek == DayOfWeek.Monday)
            {

                return System.Windows.Visibility.Visible;
            }
            else
                return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class weekNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var day = value as Day;
            if (day != null)
            {
                int weeknumber = day.DT.DayOfYear / 7 + 2;

                return "  "+AppResources.KW+" " + weeknumber + "  ";
            }
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


}
