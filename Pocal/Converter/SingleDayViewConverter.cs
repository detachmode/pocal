using Pocal.Helper;
using Pocal.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Windows.ApplicationModel.Appointments;
using Cimbalino.Toolkit.Converters;
using System.Windows.Media;
using System.Windows.Controls;

namespace Pocal.Converter
{

    internal static class cultureSettings
    {
        public static CultureInfo ci = new CultureInfo("de-DE");
    }


    public class windowHeaderDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var day = value as Day;
            if (day != null)
            {

                DateTime dt = day.DT;
                if (dt.Date == DateTime.Now.Date)
                {
                    return "Heute";

                }
                return dt.ToString("dddd", cultureSettings.ci) + ", " + dt.ToString("M", cultureSettings.ci);

            }
            else return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }
    }

    public class NullOrWhiteSpaceCollapser : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PocalAppointment pa = value as PocalAppointment;
            if (String.IsNullOrWhiteSpace(pa.Location))
            {
                return System.Windows.Visibility.Collapsed;
            }
            return System.Windows.Visibility.Visible;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }



    public class DetailNotesCollapser : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PocalAppointment pa = value as PocalAppointment;
            if (String.IsNullOrWhiteSpace(pa.Location) || pa.Duration > TimeSpan.FromHours(1.2))
            {
                return System.Windows.Visibility.Visible;
            }
            return System.Windows.Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class revertBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                if ((bool)value)
                    return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }

    }


    public class singelDayApptHeight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var appt = (value as Appointment);
            if (appt != null)
            {
                if (appt.AllDay)
                    return 0;

                double result = (appt.Duration.Hours) * HourLine.Height;

                if (appt.Duration.Minutes != 0)
                    result += HourLine.Height / 2;

                if (appt.StartTime.Minute != 0)
                    result += HourLine.Height / 2;

                if ((appt.Duration.Hours) == 0)
                    result = HourLine.Height / 2;



                return result + 1;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }

    }

    public class singelDayApptWidth : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int fullWidth = 406;
            int columnsCount = (int)value;
            if (columnsCount > 1)
            {
                if (columnsCount > 4)
                    columnsCount = 4;

                return fullWidth / columnsCount + 2;
            }
            return (fullWidth + 4); // 2 = BorderSize
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }

    }


    public class singelDayApptRectangleMargin : MultiValueConverterBase
    {
        private Thickness margin = new Thickness(0, 0, 0, 0);
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || values[1] == null)
                return margin;
            DateTimeOffset starttime = (DateTimeOffset)values[0];
            TimeSpan duration = (TimeSpan)values[1];
            DateTimeOffset endtime = starttime + duration;

            int x = starttime.Minute % 30;
            if (x == 0)
                x = 0;
            margin.Top = 1.16 * x;

            x = endtime.Minute % 30;
            if (x == 0)
                x = 30;
            margin.Bottom = 1.16 * (30 - x);

            return margin;
        }

        public override object[] ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }




    public class singelDayApptTranslate : MultiValueConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TranslateTransform myTranslate = new TranslateTransform();
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myTranslate);

            if (values[0] == null || values[1] == null || values[2] == null)
            {
                return myTransformGroup;
            }

            int conflicts = (int)values[0];
            int column = (int)values[1];
            DateTimeOffset starttime = (DateTimeOffset)values[2];

            if (conflicts > 4)
                conflicts = 4;

            if (conflicts == 0)
                conflicts = 1;

            
            calcY(starttime, myTranslate);
            calcX(conflicts, column, myTranslate);


            
            


            return myTransformGroup;





        }

        private static void calcY(DateTimeOffset starttime, TranslateTransform myTranslate)
        {
            double value = 0;

            value = (starttime.Hour * HourLine.Height);


            if (starttime.Minute > 30)
            {
                value = value + HourLine.Height / 2;

            }

            myTranslate.Y = value;

        }

        private static void calcX(int conflicts, int column, TranslateTransform myTranslate)
        {
            myTranslate.X = 406 / conflicts * (column - 1);
        }

        public override object[] ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }





}


