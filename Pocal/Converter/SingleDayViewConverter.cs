namespace Pocal.Converter
{
	using Pocal.Model;
	using System;
	using System.Globalization;
	using System.Windows.Data;
	using System.Windows.Media;
	using Windows.ApplicationModel.Appointments;

	internal static class cultureSettings
	{
		public static CultureInfo ci = new CultureInfo("de-DE");
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


