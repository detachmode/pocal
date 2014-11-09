using Pocal.Helper;
using Pocal.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Windows.ApplicationModel.Appointments;
using Cimbalino.Toolkit.Converters;
using System.Windows.Media;

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
                else
                    return dt.ToString("dddd", cultureSettings.ci) + ", " + dt.ToString("M", cultureSettings.ci);

            }
            else return null;
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
            else
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
            else
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
                    result += HourLine.Height/2;

                if (appt.StartTime.Minute != 0)
                    result += HourLine.Height / 2;

                if ((appt.Duration.Hours) == 0)
                    result = HourLine.Height / 2 ;

                    

                return result +1;
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
            int maxConflicts = (int)value;
            if (maxConflicts != 0)
            {
                if (maxConflicts > 4)
                    maxConflicts = 4;
                
                return fullWidth / maxConflicts+2;
            }
            return fullWidth +2; // 2 = BorderSize
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }

    }


    public class singelDayApptTranslate : MultiValueConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || values[1] == null || values[2] == null)
            {
                return null;
            }

            int conflicts = (int)values[0];
            int column = (int)values[1];
            DateTimeOffset starttime = (DateTimeOffset)values[2];

            if (conflicts > 4)
                conflicts = 4;


            TranslateTransform myTranslate = new TranslateTransform();
            calcY(starttime, myTranslate);
            calcX(conflicts, column, myTranslate);


            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myTranslate);


            return myTransformGroup;





        }

        private static void calcY(DateTimeOffset starttime, TranslateTransform myTranslate)
        {
            double value = 0;

            value = (starttime.Hour * HourLine.Height);
            

            if (starttime.Minute > 30)
            {
                value = value + HourLine.Height/2;

            }
            
            myTranslate.Y = value;
            
        }

        private static void calcX(int conflicts, int column, TranslateTransform myTranslate)
        {
            myTranslate.X = 406 / conflicts * (column - 1);
        }

        public override object[] ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }





}


