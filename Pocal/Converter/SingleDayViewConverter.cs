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

                int half = 0;
                if (appt.Duration.Minutes >= 30)
                    half = 35;

                if ((appt.Duration.Hours) == 0)
                    return 36;

                return ((appt.Duration.Hours) * 70 + half + 1);
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
            if (maxConflicts != null && maxConflicts != 0)
            {
                if (maxConflicts > 4)
                    maxConflicts = 4;
                
                return fullWidth / maxConflicts;
            }
            return fullWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }

    }


    public class singelDayApptTranslateX : MultiValueConverterBase
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
            myTranslate.Y = (starttime.Hour * 70); ;
            myTranslate.X = 406 / conflicts * (column - 1);


            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myTranslate);


            return myTransformGroup;





        }

        public override object[] ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }





}


