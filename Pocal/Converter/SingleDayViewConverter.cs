﻿using Pocal.Model;
using Pocal.ViewModel;
using System;
using System.Globalization;
using System.Windows.Data;
using Windows.ApplicationModel.Appointments;

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


	public class singelDayApptTranslateY : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var appt = (value as PocalAppointment).Appt;
			if (appt != null)
			{
				return (appt.StartTime.Hour - App.ViewModel.SingleDayViewModel.FirstHour) * 70 - 1;	
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
			var appt = (value as PocalAppointment).Appt;
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

