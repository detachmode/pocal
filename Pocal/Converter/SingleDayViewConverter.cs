using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Windows.ApplicationModel.Appointments;
using Cimbalino.Toolkit.Converters;
using Pocal.Helper;
using Pocal.Resources;
using Pocal.ViewModel;

namespace Pocal.Converter
{
    public static class CultureSettings
    {
        public static CultureInfo Ci = CultureInfo.CurrentUICulture;
    }


    public class SDV_Background : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush((Color) Application.Current.Resources["SDV_BG"]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class WindowHeaderDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt;
            dt = (DateTime) value;

            if (dt.Year == 1)
            {
                return "";
            }

            if (dt.Date == DateTime.Now.Date)
            {
                return AppResources.today;
            }
            return dt.ToString("dddd", CultureSettings.Ci) + ", " + dt.ToString("M", CultureSettings.Ci);
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
            var pa = value as PocalAppointment;
            return pa != null && string.IsNullOrWhiteSpace(pa.Location) ? Visibility.Collapsed : Visibility.Visible;
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
            var pa = value as PocalAppointment;
            if (pa == null) return Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(pa.Location) || pa.Duration > TimeSpan.FromHours(1.2))
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class RevertBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return Visibility.Collapsed;
            if ((bool) value)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class SingelDayApptHeight : IValueConverter
    {
        private Appointment _appt;
        private DateTimeOffset _endTime;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var apptTest = (value as Appointment);
            if (apptTest == null) return 0;
            if (apptTest.AllDay) return 0;

            _appt = apptTest;
            double result;

            _endTime = _appt.StartTime + _appt.Duration;

            if (ApptBeginsAndEndsThisDay())
            {
                var startTimeSnapped30 = _appt.StartTime.AddMinutes(-_appt.StartTime.Minute%30);
                DateTimeOffset endTimeSnapped30;
                if (_endTime.Minute != 0 && _endTime.Minute != 30)
                    endTimeSnapped30 = _endTime.AddMinutes(+(30 - _endTime.Minute%30));
                else
                    endTimeSnapped30 = _endTime;

                var durationSnapped30 = endTimeSnapped30 - startTimeSnapped30;
                var duration = (durationSnapped30.TotalMinutes/30);

                result = duration*HourLine.Height/2.0;
                return result + 2;
            }


            if (ApptJustBeginsThisDay())
            {
                result = (HourLine.Height*24 + 1);
                result -= (_appt.StartTime.Hour)*HourLine.Height;
                if (_appt.StartTime.Minute >= 30)
                    result -= HourLine.Height/2.0;
                return result + 2;
            }

            if (ApptJustEndsThisDay())
            {
                result = (_endTime.Hour*HourLine.Height);
                if (_endTime.Minute > 0)
                    result += HourLine.Height/2.0;
                if (_endTime.Minute > 29)
                    result += HourLine.Height/2.0;
                return result + 2;
            }

            var completeDayHeight = (HourLine.Height*24);
            return completeDayHeight + 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        private bool ApptBeginsAndEndsThisDay()
        {
            var testDate = App.ViewModel.SingleDayViewModel.TappedDay.Dt.Date;
            return (_appt.StartTime.Date == testDate && _endTime.Date == testDate);
        }

        private bool ApptJustEndsThisDay()
        {
            return (_appt.StartTime.Date != App.ViewModel.SingleDayViewModel.TappedDay.Dt.Date
                    && _endTime.Date == App.ViewModel.SingleDayViewModel.TappedDay.Dt.Date);
        }

        private bool ApptJustBeginsThisDay()
        {
            return (_endTime.Date != App.ViewModel.SingleDayViewModel.TappedDay.Dt.Date
                    && _appt.StartTime.Date == App.ViewModel.SingleDayViewModel.TappedDay.Dt.Date);
        }
    }

    public class SingelDayApptWidth : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var screenSizeMultiplicator = App.DisplayInformationEmulator.DisplayInformationEx.ViewPixelsPerHostPixel;
            var fullWidth = (480*screenSizeMultiplicator - 73);
            var columnsCount = (int) value;

            if (columnsCount <= 1) return (fullWidth + screenSizeMultiplicator*2); // 2 = BorderSize

            if (columnsCount > 4)
                columnsCount = 4;

            return fullWidth/columnsCount + screenSizeMultiplicator*1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class SingelDayApptRectangleMargin : MultiValueConverterBase
    {
        private Thickness _margin = new Thickness(0, 0, 0, 0);

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || values[1] == null)
                return _margin;
            var starttime = (DateTimeOffset) values[0];
            var duration = (TimeSpan) values[1];
            var endtime = starttime + duration;

            var x = starttime.Minute%30;
            if (x == 0)
                x = 0;
            _margin.Top = 1.16*x;

            x = endtime.Minute%30;
            if (x == 0)
                x = 30;
            _margin.Bottom = 1.16*(30 - x);

            return _margin;
        }

        public override object[] ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class SingelDayApptTranslate : MultiValueConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var myTranslate = new TranslateTransform();
            var myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myTranslate);

            if (values[0] == null || values[1] == null || values[2] == null)
            {
                return myTransformGroup;
            }

            var conflicts = (int) values[0];
            var column = (int) values[1];
            var starttime = (DateTimeOffset) values[2];

            if (conflicts > 4)
                conflicts = 4;

            if (conflicts == 0)
                conflicts = 1;


            CalcY(starttime, myTranslate);
            CalcX(conflicts, column, myTranslate);


            return myTransformGroup;
        }

        private static void CalcY(DateTimeOffset starttime, TranslateTransform myTranslate)
        {
            double value;

            if (starttime.Date < App.ViewModel.SingleDayViewModel.TappedDay.Dt.Date)
            {
                myTranslate.Y = 0;
                return;
            }

            value = (starttime.Hour*HourLine.Height);


            if (starttime.Minute >= 30)
            {
                value = value + (HourLine.Height/2.0);
            }

            myTranslate.Y = value;
        }

        private static void CalcX(int conflicts, int column, TranslateTransform myTranslate)
        {
            var screenSizeMultiplicator = App.DisplayInformationEmulator.DisplayInformationEx.ViewPixelsPerHostPixel;
            myTranslate.X = (480*screenSizeMultiplicator - 73)/conflicts*(column - 1);
        }

        public override object[] ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}