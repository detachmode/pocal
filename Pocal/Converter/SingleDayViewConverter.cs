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

    internal static class CultureSettings
    {
        public static CultureInfo ci = new CultureInfo("de-DE");
    }




    public class SDV_Background : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {


            return new SolidColorBrush((Color)App.Current.Resources["SDV_BG"]);

              
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class windowHeaderDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dt = (DateTime)value;
            if (dt != null)
            {

                if (dt.Year == 1)
                {
                    return "";
                }
                //DateTime dt = day.DT;
                if (dt.Date == DateTime.Now.Date)
                {
                    return "Heute";

                }
                return dt.ToString("dddd", CultureSettings.ci) + ", " + dt.ToString("M", CultureSettings.ci);

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
        private Appointment appt;
        private DateTimeOffset endTime;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var apptTest = (value as Appointment);
            if (apptTest != null)
            {
                if (apptTest.AllDay) return 0;

                appt = apptTest;
                double result;
                endTime = (DateTimeOffset)appt.StartTime + appt.Duration;

                if (apptBeginsAndEndsThisDay())
                {
                    result = (appt.Duration.Hours) * HourLine.Height;
                    if (appt.StartTime.Minute > 0 && endTime.Minute > 30)
                    {
                        result += HourLine.Height;
                        return result;
                    }
                    if (appt.Duration.Minutes != 0)
                        result += HourLine.Height / 2;

                    if (appt.StartTime.Minute != 0)
                        result += HourLine.Height / 2;

                    if ((appt.Duration.Hours) == 0)
                        result = HourLine.Height / 2;
                    return result +2;
                }

                
                if (apptJustBeginsThisDay())              
                {
                    result = (HourLine.Height * 24 +1) ;
                    result -= (appt.StartTime.Hour)*HourLine.Height;
                    return result +2;
                }

                if (apptJustEndsThisDay())
                {
                    result = (endTime.Hour * HourLine.Height);
                    if (endTime.Minute != 0)
                        result += HourLine.Height / 2;
                    return result + 2;

                }

                var completeDayHeight = (HourLine.Height * 24);
                return completeDayHeight + 2;

            }
            return 0;
        }

        private bool apptBeginsAndEndsThisDay()
        {
            var testDate = App.ViewModel.SingleDayViewModel.TappedDay.DT.Date;
            return (appt.StartTime.Date == testDate && endTime.Date == testDate);
        }
        
        private bool apptJustEndsThisDay()
        {
            return (appt.StartTime.Date != App.ViewModel.SingleDayViewModel.TappedDay.DT.Date 
                && endTime.Date == App.ViewModel.SingleDayViewModel.TappedDay.DT.Date);
        }

        private bool apptJustBeginsThisDay()
        {
            return (endTime.Date != App.ViewModel.SingleDayViewModel.TappedDay.DT.Date
                 && appt.StartTime.Date == App.ViewModel.SingleDayViewModel.TappedDay.DT.Date);
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
            double screenSizeMultiplicator = App.DisplayInformationEmulator.DisplayInformationEx.ViewPixelsPerHostPixel;
            double fullWidth = 421 * screenSizeMultiplicator;
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

            if (starttime.Date < App.ViewModel.SingleDayViewModel.TappedDay.DT.Date)
            {
                myTranslate.Y = 0;
                return;
            }

            value = (starttime.Hour * HourLine.Height);


            if (starttime.Minute >= 30)
            {
                value = value + HourLine.Height / 2;

            }

            myTranslate.Y = value;

        }

        private static void calcX(int conflicts, int column, TranslateTransform myTranslate)
        {
            double screenSizeMultiplicator = App.DisplayInformationEmulator.DisplayInformationEx.ViewPixelsPerHostPixel;
            myTranslate.X = 421*screenSizeMultiplicator / conflicts * (column - 1);
        }

        public override object[] ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }





}


