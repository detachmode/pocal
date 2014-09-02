using Pocal.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.ApplicationModel.Appointments;

namespace Pocal.ViewModel
{
	public static class AppointmentProvider
	{
		private static AppointmentStore appointmentStore;
		private static IReadOnlyList<AppointmentCalendar> calendars;

		public static async void reloadPocalApptsAndDays()
		{
			var allAppts = await getAllAppts();

			if (allAppts.Any())
			{
				calendars = await appointmentStore.FindAppointmentCalendarsAsync();

				reloadPocalAppts(allAppts);

				fillDayview();
				updateTappedDay();
			}
		}

		private static async Task<IReadOnlyList<Appointment>> getAllAppts()
		{
			await setAppointmentStore();
			FindAppointmentsOptions findOptions = getFindOptions();

			var allAppts = await appointmentStore.FindAppointmentsAsync(DateTime.Now.Date, TimeSpan.FromDays(App.ViewModel.howManyDays), findOptions);
			return allAppts;
		}

		private static void reloadPocalAppts(IReadOnlyList<Appointment> allAppts)
		{
			App.ViewModel.PocalAppts.Clear();

			foreach (var appt in allAppts)
			{
				var cal = calendars.First(c => c.LocalId == appt.CalendarId);
				var calColor = new System.Windows.Media.Color() { A = cal.DisplayColor.A, B = cal.DisplayColor.B, R = cal.DisplayColor.R, G = cal.DisplayColor.G };

				PocalAppointment pocalAppt = new PocalAppointment { Appt = appt, CalColor = new SolidColorBrush(calColor) };
				App.ViewModel.PocalAppts.Add(pocalAppt);
			}
		}

		private static FindAppointmentsOptions getFindOptions()
		{
			FindAppointmentsOptions findOptions = new FindAppointmentsOptions();
			//findOptions.MaxCount = 100;
			findOptions.FetchProperties.Add(AppointmentProperties.Subject);
			findOptions.FetchProperties.Add(AppointmentProperties.Location);
			findOptions.FetchProperties.Add(AppointmentProperties.StartTime);
			findOptions.FetchProperties.Add(AppointmentProperties.AllDay);
			findOptions.FetchProperties.Add(AppointmentProperties.Duration);
			return findOptions;
		}

		private static async System.Threading.Tasks.Task setAppointmentStore()
		{
			if (appointmentStore == null)
				appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);
		}

		private static void updateTappedDay()
		{

			if (App.ViewModel.SingleDayViewModel.TappedDay == null)
			{
				App.ViewModel.SingleDayViewModel.TappedDay = App.ViewModel.Days[0];
			}
			else
			{
				DateTime dt = App.ViewModel.SingleDayViewModel.TappedDay.DT;
				Day td = App.ViewModel.Days.First(x => x.DT.Date == dt.Date);
				App.ViewModel.SingleDayViewModel.TappedDay = td;
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
					PocalApptsOfDay = PocalAppointmentManager.getPocalApptsOfDay(dt)
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



	}
}
