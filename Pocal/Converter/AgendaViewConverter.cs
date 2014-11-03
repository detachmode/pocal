using Pocal.ViewModel;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Windows.ApplicationModel.Appointments;
using System.Linq;

namespace Pocal.Converter
{

    public class weekConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                DateTime time = (DateTime)value;
                // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
                // be the same week# as whatever Thursday, Friday or Saturday are,
                // and we always get those right
                DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
                if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
                {
                    time = time.AddDays(3);
                }

                // Return the week of our adjusted day
                int str = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                return "Woche " + str;

            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }


    }

    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {

                DateTime dt = (DateTime)value;
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


    public class CurrentTopWeekConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Day)
            {
                TimeSpan ts = ((Day)value).DT.Date - DateTime.Now.Date;

                if (ts.Days < 7)
                {
                    return "";
                }
                else
                {
                    int delta = (int)ts.Days / 7;
                    return "in " + delta.ToString() + " Wochen";
                }


            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }


    }

    public class CurrentTopDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Day)
            {

                TimeSpan ts = ((Day)value).DT.Date - DateTime.Now.Date;
                int delta = ts.Days % 7;
                return "und " + delta.ToString() + " Tagen";



            }
            else return null;
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
            return null;
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
            return null;
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
                //System.Diagnostics.Debug.WriteLine(" " + appt.Subject + " " + appt.Duration.Days.ToString()+ "\n AllDay: "+appt.AllDay.ToString());
                //Zeit
                if (appt.AllDay || appt.Duration == TimeSpan.FromDays(1))
                {
                    int d = ((appt.StartTime + appt.Duration).Date - appt.StartTime.Date).Days;
                    if (d == 1)
                    {
                        str += d + " Tag";
                    }
                    else
                    {
                        str += d + " Tage";
                    }

                }
                else
                {
                    str += appt.StartTime.Hour.ToString();
                    str += ":";
                    str += appt.StartTime.Minute.ToString("00");

                    str += " - ";

                    str += (appt.StartTime + appt.Duration).DateTime.Hour.ToString();
                    str += ":";
                    str += (appt.StartTime + appt.Duration).DateTime.Minute.ToString("00");
                    //str = ts.Hours + " Stunde";

                }

                // Location
                if (String.IsNullOrWhiteSpace(appt.Location))
                {
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
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }



    }

    public class subjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var appt = (value as Appointment);
            var subject = appt.Subject;
            if (subject != null)
            {
                return subject;
            }
            else return "privat";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return null;
        }


    }

    public class highlightedDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var day = value as Day;
            if ((day.DT.DayOfWeek == DayOfWeek.Saturday) || (day.DT.DayOfWeek == DayOfWeek.Sunday))
            {
                return new SolidColorBrush(Color.FromArgb(255, 40, 40, 40));
            }
            else
                return new SolidColorBrush(Color.FromArgb(0, 255, 0, 0));
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

                return "||||||||||||||||| WOCHE " + weeknumber +" ||||||";
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


}
