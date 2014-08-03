using Pocal.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

			IReadOnlyList<Appointment> appointments =
				await appointmentStore.FindAppointmentsAsync(DateTime.Now, TimeSpan.FromDays(days), findOptions);


			App.ViewModel.Appts.Clear();
			foreach (Appointment a in appointments)
			{
				App.ViewModel.Appts.Add(a);
			}

			fillDayview();

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

		public static ObservableCollection<Appointment> getApptsOfDay(DateTime dt)
		{

			ObservableCollection<Appointment> thisDayAppts = new ObservableCollection<Appointment>();
			foreach (Appointment a in App.ViewModel.Appts)
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

		private static ObservableCollection<Appointment> sortAppointments(ObservableCollection<Appointment> appts)
		{
			ObservableCollection<Appointment> sorted = new ObservableCollection<Appointment>();
			IEnumerable<Appointment> query = appts.OrderBy(appt => appt.StartTime);

			foreach (Appointment appt in query)
			{
				sorted.Add(appt);
			}

			return sorted;
		}

	}
}
