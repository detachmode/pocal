using Pocal.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;

namespace Pocal
{
	public class PocalAppointmentManager
	{

		public async void editAppointment(PocalAppointment pocalappt)
		{
			var store = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);

			if (pocalappt.Appt.OriginalStartTime == null)
			{
				await store.ShowAppointmentDetailsAsync(pocalappt.Appt.LocalId);
				var appt = await store.GetAppointmentAsync(pocalappt.Appt.LocalId);

				updatePocalAppt(appt, pocalappt);

			}
			else
			{
				await store.ShowAppointmentDetailsAsync(pocalappt.Appt.LocalId, pocalappt.Appt.OriginalStartTime.Value);
				var appt = await store.GetAppointmentInstanceAsync(pocalappt.Appt.LocalId, pocalappt.Appt.OriginalStartTime.Value);

				updatePocalAppt(appt, pocalappt);
			}
		}


		private void updatePocalAppt(Appointment appt, PocalAppointment pocalappt)
		{
			if (appt == null)
			{
				App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay.Remove(pocalappt);
				Pocal.ViewModel.AppointmentProvider.ShowUpcomingAppointments();
				return ;
			}

			if (isOnlyApptOfTappedDay(appt))
			{
				int indexInTappedDayAppts = App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay.IndexOf(pocalappt);

				App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay[indexInTappedDayAppts].PropertyChanged += PocalApptPropertyChanged;
				App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay[indexInTappedDayAppts].Appt = appt;
				App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay[indexInTappedDayAppts].PropertyChanged -= PocalApptPropertyChanged;
			}
			else
			{
				App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay.Remove(pocalappt);
				Pocal.ViewModel.AppointmentProvider.ShowUpcomingAppointments();
			}
		}



		private bool isOnlyApptOfTappedDay(Appointment appt)
		{
			var day = App.ViewModel.SingleDayViewModel.TappedDay;
			var endTime = (appt.StartTime + appt.Duration);

			if (appt.Recurrence != null)
				return false;

			return (!(appt.StartTime.Date > day.DT.Date) && !(endTime.Date < day.DT.Date));

		}


		private void PocalApptPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{

			if (e.PropertyName == "Duration" || e.PropertyName == "StartTime")
			{
				System.Diagnostics.Debug.WriteLine("updatePocalApptsOfDays");
				Pocal.ViewModel.AppointmentProvider.ShowUpcomingAppointments();
			}


		}
	}
}
