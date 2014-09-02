using Pocal.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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


		#region get PocalAppointments of one Day

		public static ObservableCollection<PocalAppointment> getPocalApptsOfDay(DateTime dt)
		{

			ObservableCollection<PocalAppointment> thisDayAppts = new ObservableCollection<PocalAppointment>();
			foreach (PocalAppointment pocalAppt in App.ViewModel.PocalAppts)
			{
				if (pocalAppt.StartTime.Day == dt.Day)
				{
					thisDayAppts.Add(pocalAppt);
					//a.StartTime.Minute
				}
			}
			// Sort 
			thisDayAppts = sortAppointments(thisDayAppts);

			return thisDayAppts;
		}

		private static ObservableCollection<PocalAppointment> sortAppointments(ObservableCollection<PocalAppointment> PocalAppts)
		{
			ObservableCollection<PocalAppointment> sorted = new ObservableCollection<PocalAppointment>();
			IEnumerable<PocalAppointment> query = PocalAppts.OrderBy(appt => appt.StartTime);

			foreach (PocalAppointment appt in query)
			{
				sorted.Add(appt);
			}

			return sorted;
		}

		#endregion




		public async void addAppointment(DateTime starttime)
		{

			var appointment = new Windows.ApplicationModel.Appointments.Appointment();

			appointment.StartTime = starttime;
			appointment.Duration = TimeSpan.FromHours(1);
			appointment.Reminder = TimeSpan.FromMinutes(15);

			String appointmentId = await AppointmentManager.ShowEditNewAppointmentAsync(appointment);

			//todo
			Pocal.ViewModel.AppointmentProvider.reloadPocalApptsAndDays();

		}



		#region Edit Appointments


		public async void editAppointment(PocalAppointment pocalappt)
		{
			await setAppointmentStore();

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

		private async Task setAppointmentStore()
		{
			if (appointmentStore == null)
				appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);
		}

		private void updatePocalAppointment(Appointment appt, PocalAppointment pocalappt)
		{

			removeIfApptIsNull(appt, pocalappt);

			if (isCompleteRefreshNeeded(appt))
			{
				System.Diagnostics.Debug.WriteLine("CompleteRefresh");
				Pocal.ViewModel.AppointmentProvider.reloadPocalApptsAndDays();
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

			// Bei sich wiederholenden PocalAppts wird immer refresht
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
		#endregion

	}
}
