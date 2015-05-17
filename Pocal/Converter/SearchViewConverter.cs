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
                return dt.ToString("dddd", CultureSettings.Ci) + ", " + dt.ToString("M", CultureSettings.Ci);

            var str = AppResources.today;
            return str.First().ToString().ToUpper() + str.Substring(1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
