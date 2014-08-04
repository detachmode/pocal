using Pocal.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Appointments;

namespace Pocal.ViewModel
{
	public static class AppointmentProvider
	{
		private static AppointmentStore appointmentStore;

		public static async void ShowUpcomingAppointments()
		{
			int days = App.ViewModel.howManyDays;
			if (appointmentStore == null)
			{
				appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);
			}

			FindAppointmentsOptions findOptions = new FindAppointmentsOptions();
			findOptions.MaxCount = 30;
			findOptions.FetchProperties.Add(AppointmentProperties.Subject);
			findOptions.FetchProperties.Add(AppointmentProperties.Location);
			findOptions.FetchProperties.Add(AppointmentProperties.StartTime);
			findOptions.FetchProperties.Add(AppointmentProperties.Duration);

			IReadOnlyList<Appointment> allAppts =
				await appointmentStore.FindAppointmentsAsync(DateTime.Now, TimeSpan.FromDays(days), findOptions);

			if (allAppts.Any())
			{
				var calendars = await appointmentStore.FindAppointmentCalendarsAsync();

				App.ViewModel.AllPocalAppointments.Clear();
				foreach (Appointment appt in allAppts)
				{
					var cal = calendars.First( c => c.LocalId == appt.CalendarId);
					var calColor = new System.Windows.Media.Color {A=cal.DisplayColor.A, B = cal.DisplayColor.B, R = cal.DisplayColor.R, G = cal.DisplayColor.G};

					App.ViewModel.AllPocalAppointments.Add(new PocalAppointment(appt, calColor));
				}

				fillDayview();
			}



			if (App.ViewModel.SingleDayViewModel.TappedDay == null)
			{
				App.ViewModel.SingleDayViewModel.TappedDay = App.ViewModel.Days[0];
			}


		}


		private static void fillDayview()
		{
			DateTime dt = DateTime.Now;
			CultureInfo ci = new CultureInfo("de-DE");

			App.ViewModel.Days.Clear();
			for (int i = 0; i < App.ViewModel.howManyDays; i++)
			{
				// Create New Day with its Appointments
				App.ViewModel.Days.Add(new Day()
				{
					DT = dt,
					DayAppts = getApptsOfDay(dt)
				});

				// Sunday Attribute
				if (dt.DayOfWeek == DayOfWeek.Sunday)
				{
					App.ViewModel.Days[i].Sunday = true;
				}

				// Iteration ++ ein Tag
				dt = dt.AddDays(1);

			}
		}

		public static ObservableCollection<PocalAppointment> getApptsOfDay(DateTime dt)
		{

			ObservableCollection<PocalAppointment> thisDayAppts = new ObservableCollection<PocalAppointment>();
			foreach (PocalAppointment a in App.ViewModel.AllPocalAppointments)
			{
				if (a.StartTime.Day == dt.Day)
				{
					thisDayAppts.Add(a);
					//a.StartTime.Minute
				}
			}
			// Sort 
			thisDayAppts = sortAppointments(thisDayAppts);

			return thisDayAppts;
		}

		private static ObservableCollection<PocalAppointment> sortAppointments(ObservableCollection<PocalAppointment> appts)
		{
			ObservableCollection<PocalAppointment> sorted = new ObservableCollection<PocalAppointment>();
			IEnumerable<PocalAppointment> query = appts.OrderBy(appt => appt.StartTime);

			foreach (PocalAppointment appt in query)
			{
				sorted.Add(appt);
			}

			return sorted;
		}

	}
}
