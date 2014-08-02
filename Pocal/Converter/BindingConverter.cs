
using Pocal.ViewModelBinders;
using Microsoft.Phone.Controls;
//using Microsoft.Phone.UserData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Windows.ApplicationModel.Appointments;

namespace Pocal.Converter
{
	public static class cultureSettings
	{
		public static CultureInfo ci = new CultureInfo("de-DE");
	}


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
			var appt = value as Appointment;
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
			var appt = value as Appointment;
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

	public class allDayStrokeWidthConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool)
			{

				if ((bool)value == true)
				{

					return 15.0;
				}
				else
					return 0.0;


			}
			else return null;
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
			var appt = value as Appointment;
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
				if (appt.Location == "")
				{
					return str;
				}
				else
					return str + " (" + appt.Location + ")";
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
			if (value != null)
			{
				return value;
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
				return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}




	public class singelDayApptTranslateY : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var appt = value as Appointment;
			if (appt != null)
			{
				return (appt.StartTime.Hour-8)*70-1;	
			}
			return 0;
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
			var appt = value as Appointment;
			if (appt != null)
			{
				if (appt.AllDay)
					return 0;

				int half = 0;
				if (appt.Duration.Minutes>=30)
					half = 35;
				
				if ((appt.Duration.Hours) == 0)
					return 36;

				return ((appt.Duration.Hours)*70 +half+1);	
			}
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{

			return null;
		}


	}
	

}


