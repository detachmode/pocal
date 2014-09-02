using Pocal.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel.Appointments;

namespace Pocal
{
	public class PocalAppointmentManager
	{
		private AppointmentStore appointmentStore;
		private async void setAppointmentStore()
		{
			if (appointmentStore == null)
				appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);
		}


			
		public async void editAppointment(PocalAppointment pocalappt)
		{
			//setAppointmentStore();
			if (appointmentStore == null)
				appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);

			if (pocalappt.Appt.OriginalStartTime == null)
			{
				await appointmentStore.ShowAppointmentDetailsAsync(pocalappt.Appt.LocalId);
				var appt = await appointmentStore.GetAppointmentAsync(pocalappt.Appt.LocalId);

				updatePocalAppointment(appt, pocalappt);

			}
			else
			{
				await appointmentStore.ShowAppointmentDetailsAsync(pocalappt.Appt.LocalId, pocalappt.Appt.OriginalStartTime.Value);
				var appt = await appointmentStore.GetAppointmentInstanceAsync(pocalappt.Appt.LocalId, pocalappt.Appt.OriginalStartTime.Value);

				updatePocalAppointment(appt, pocalappt);
			}
		}



		public async void addAppointment(DateTime starttime)
		{
			
			var appointment = new Windows.ApplicationModel.Appointments.Appointment();

			appointment.StartTime =  starttime;
			appointment.Duration = TimeSpan.FromHours(1);
			appointment.Reminder = TimeSpan.FromMinutes(15);

			String appointmentId = await Windows.ApplicationModel.Appointments.AppointmentManager.ShowEditNewAppointmentAsync(appointment);

			//todo
			Pocal.ViewModel.AppointmentProvider.ShowUpcomingAppointments();

		}


		private void updatePocalAppointment(Appointment appt, PocalAppointment pocalappt)
		{

			removeIfApptIsNull(appt, pocalappt);

			if (isCompleteRefreshNeeded(appt))
			{
				System.Diagnostics.Debug.WriteLine("CompleteRefresh");
				Pocal.ViewModel.AppointmentProvider.ShowUpcomingAppointments();
			}
			else
			{
				int indexInTappedDayAppts = App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay.IndexOf(pocalappt);
				App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay[indexInTappedDayAppts].Appt = appt;
			}
		}


		private void removeIfApptIsNull(Appointment appt, PocalAppointment pocalappt)
		{
			if (appt == null)
				App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay.Remove(pocalappt);
		}


		private bool isCompleteRefreshNeeded(Appointment appt)
		{
			
			// Appointment wurde gelöscht -> auch refresh
			if (appt == null)
				return true;

			// Bei sich wiederholenden Appts wird immer refresht
			if (appt.Recurrence != null)
				return true;

			// nur refresh wenn der Termin sich innerhalb des tappedDays befindet
			return !(isOnlyApptOfTappedDay(appt));

		}

		private bool isOnlyApptOfTappedDay(Appointment appt)
		{
			var day = App.ViewModel.SingleDayViewModel.TappedDay;
			var endTime = (appt.StartTime + appt.Duration);

			return ((appt.StartTime.Date == day.DT.Date) && (endTime.Date == day.DT.Date));
		}

	}
}
