using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Windows.ApplicationModel.Appointments;
using Pocal.Helper;
using Pocal.Resources;
using Pocal.ViewModel;

namespace Pocal.Converter
{
    public static class ConverterBrushes
    {
        public static SolidColorBrush PastDays = new SolidColorBrush(Color.FromArgb(255, 170, 170, 170));
        public static SolidColorBrush NoWeekendHeader = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        public static SolidColorBrush DarkGray = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
        public static SolidColorBrush Black = new SolidColorBrush(Colors.Black);
        public static SolidColorBrush Red = new SolidColorBrush(Colors.Red);
    }


    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime)) return "";

            var dt = (DateTime) value;
            if (dt.Date != DateTime.Now.Date)
                return dt.ToString("dddd", CultureSettings.Ci) + ", " + dt.ToString("M", CultureSettings.Ci);

            var str = AppResources.today;
            return str.First().ToString().ToUpper() + str.Substring(1);
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
            if (!(value is Day)) return "";
            var dt = ((Day) value).Dt;
            return DeltaTimeStringCreator.GetDeltaTime(dt);
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
            if (!(value is DateTime)) return Visibility.Collapsed;

            var ts = ((DateTime) value).Date - DateTime.Now.Date;

            if (ts.Days%7 == 0 || Math.Abs(ts.Days) < 7)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class HourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pocalAppointment = value as PocalAppointment;
            if (pocalAppointment == null) return null;

            var appt = pocalAppointment.Appt;
            if (appt == null) return "";

            if (appt.AllDay)
            {
                return "";
            }
            return appt.StartTime.Hour;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class MinuteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pocalAppointment = value as PocalAppointment;
            if (pocalAppointment == null) return null;

            var appt = pocalAppointment.Appt;
            if (appt == null) return "";

            if (appt.AllDay)
            {
                return "";
            }
            var i = appt.StartTime.Minute;
            return i.ToString("00");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class StartTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var appt = (value as PocalAppointment);

            if (appt == null) return "";

            // All Day ist nur im englischen kurz genug
            if (appt.AllDay || appt.Duration == TimeSpan.FromDays(1))
            {
                return AppResources.ResourceLanguage.Contains("en") ? AppResources.allDay : "";
            }

            return App.ViewModel.SettingsViewModel.IsTimeStyleAMPM() ? Convert12(appt) : Convert24(appt);

            //return AppResources.ResourceLanguage.Contains("en") ? Convert12(appt) : Convert24(appt);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        private static string Convert12(PocalAppointment appt)
        {
            var ampM = appt.StartTime.ToString("tt", CultureSettings.CiEn);
            var result = string.Format("{0:h:mm}", appt.StartTime);
            return result + " " + ampM;
        }

        private static string Convert24(PocalAppointment appt)
        {
            return string.Format("{0:HH:mm}", appt.StartTime);
        }
    }


    public class SecondLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var appt = (value as Appointment);

            if (appt == null) return "";
            var str = "";


            //Zeit
            if (appt.AllDay || appt.Duration >= TimeSpan.FromDays(1))
            {
                var d = ((appt.StartTime + appt.Duration).Date - appt.StartTime.Date).Days;
                if (d == 1)
                {
                    str += d + " " + AppResources.DaySingular;
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
            if (string.IsNullOrWhiteSpace(appt.Location))
            {
                // Notes
                if (string.IsNullOrWhiteSpace(appt.Details)) return str;
                var line1 = appt.Details.Split('\r', '\n').FirstOrDefault();
                str += " (" + line1 + ")";
            }
            else
                str += " (" + appt.Location + ")";

            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class PastDaysBackgroundInverted : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dt = (DateTime) value;
            var weeknumber = (dt.DayOfYear + 1)/7;
            return weeknumber%2 == 1 ? ConverterBrushes.DarkGray : ConverterBrushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class PastDaysBackground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dt = (DateTime) value;
            var weeknumber = (dt.DayOfYear + 1)/7;
            return weeknumber%2 == 0 ? ConverterBrushes.DarkGray : ConverterBrushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class SecondLinesForeground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dt = (DateTime) value;

            if (dt.Date < DateTime.Now.Date)
                return ConverterBrushes.PastDays;

            return (Application.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class DayHeadersForeground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dt = (DateTime) value;

            if (dt.Date < DateTime.Now.Date)
                return ConverterBrushes.PastDays;

            if (!App.ViewModel.SettingsViewModel.SundayRed)
                return (Application.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush);

            if (dt.Date.DayOfWeek == DayOfWeek.Sunday)
                return ConverterBrushes.Red;
            return (Application.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class WeekendForeground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dt = (DateTime) value;

            return (dt.DayOfWeek == DayOfWeek.Saturday) || (dt.DayOfWeek == DayOfWeek.Sunday)
                ? ConverterBrushes.PastDays
                : ConverterBrushes.NoWeekendHeader;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class WeekNumberVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var day = value as Day;
            if (day != null && day.Dt.DayOfWeek == DayOfWeek.Monday) return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class WeekNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var day = value as Day;
            if (day == null) return "";
            var weeknumber = day.Dt.DayOfYear/7 + 2;

            return "  " + AppResources.KW + " " + weeknumber + "  ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}