using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Pocal.Resources;

namespace Pocal.Converter
{
    public class DateTimeOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTimeOffset)) return "";

            var dt = (DateTimeOffset)value;
            if (dt.Date != DateTime.Now.Date)
                return dt.ToString("D", CultureSettings.Ci);

            var str = AppResources.today;
            return str.First().ToString().ToUpper() + str.Substring(1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class SearchSecondLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTimeOffset)) return "";

            var dt = (DateTimeOffset)value;
            int deltaDays = (dt.Date - DateTime.Now.Date).Days;
            if (deltaDays == 0)
            {
                 //var str = AppResources.today;
                 //return str.First().ToString().ToUpper() + str.Substring(1);
                //if (dt.Duration == TimeSpan.FromDays(1))
                //{
                //    return AppResources.ResourceLanguage.Contains("en") ? AppResources.allDay : "";
                //}

                return App.ViewModel.SettingsViewModel.IsTimeStyleAMPM() ? converterHelper.Convert12(dt) : converterHelper.Convert24(dt);
                
            }
            if (deltaDays > 0)
            {
                return (AppResources.futureTime + " " + deltaDays + " " + AppResources.DayPlural);
            }

            if (deltaDays < 0)
            {
                return (AppResources.pastTime + " " + Math.Abs(deltaDays) +" "+ AppResources.DayPlural );
            }
            return "";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    
}
