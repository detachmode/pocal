using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Shared.Helper;
using Pocal.ViewModel;
using System.Threading;

namespace Pocal.Helper
{
    public class PocalAppointmentHelper
    {

        #region NEW
        // ******** *********************** *********//
        // ******** Creates New Appointment *********//
        // ******** *********************** *********//

        public static async void addAppointment(DateTime starttime)
        {

            var appointment = newAppointmentPreset(starttime);
            await addAppointment(appointment);

        }

        public static async void addAllDayAppointment(DateTime starttime)
        {

            var appointment = newAppointmentPreset(starttime);
            appointment.AllDay = true;
            await addAppointment(appointment);

        }

        private static async Task addAppointment(Appointment appointment)
        {
            Appointment newAppointment = await CalendarAPI.Add(appointment);
            PocalAppointment newPa = await App.ViewModel.CreatePocalAppoinment(newAppointment);

            PocalAppointmentUpdater.Update(null, newPa);
        }

        private static Appointment newAppointmentPreset(DateTime starttime)
        {
            var appointment = new Windows.ApplicationModel.Appointments.Appointment();
            appointment.StartTime = starttime;
            appointment.Duration = TimeSpan.FromHours(1);
            appointment.Reminder = TimeSpan.FromMinutes(15);
            return appointment;
        }



        public static async Task<List<Appointment>> getReccurantAppointments(Appointment currentAppointment, string localID)
        {
            AppointmentStore appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);

            List<Appointment> resultList = new List<Appointment>();
            if (currentAppointment.Recurrence != null)
            {
                AppointmentCalendar calendar =
                    await appointmentStore.GetAppointmentCalendarAsync(currentAppointment.CalendarId);




                IReadOnlyList<Appointment> appointmentInstances = await
                    calendar.FindAllInstancesAsync(
                        localID,
                        DateTime.Today,
                        TimeSpan.FromDays(App.ViewModel.Days.Count),
                        CalendarAPI.GetFindOptions());

                foreach (var appt in appointmentInstances)
                {
                    resultList.Add(appt);
                }

            }
            else
                resultList.Add(currentAppointment);
            return resultList;
        }
        #endregion


        // ******** *************************** *********//
        // ******** Edits exisiting Appointment *********//
        // ******** *************************** *********//


        public static async void editAppointment(PocalAppointment pA)
        {
            Appointment appt =  await CalendarAPI.Edit(pA.Appt);

            PocalAppointment newPA = null;
            if (appt != null)
            {
                newPA = await App.ViewModel.CreatePocalAppoinment(appt);
            }
            PocalAppointmentUpdater.Update(pA, newPA);

        }




        #region TestAppointments

        public static async void AddTestAppointments()
        {
            Appointment appointment = newTestAppointment
                (
                    DateTime.Now.Date.AddHours(1.50),
                    "Einkaufen",
                    TimeSpan.FromHours(2.0)
                );
            if (appointment != null)
            {
                await addAppointment(appointment);
            }


            appointment = newTestAppointment
                (
                    DateTime.Now.Date.AddHours(4.30),
                    "Sport",
                    TimeSpan.FromHours(3.0)
                );
            await addAppointment(appointment);

            appointment = newTestAppointment
                (
                    DateTime.Now.Date.AddHours(81.30),
                    "In Urlaub fahren",
                    TimeSpan.FromHours(10.0)
                );
            await addAppointment(appointment);

            appointment = newTestAppointment
                (
                    DateTime.Now.Date.AddHours(5.50),
                    "Jeans kaufen",
                    TimeSpan.FromHours(2.0)
                );
            await addAppointment(appointment);

            appointment = newTestAppointment
                (
                    DateTime.Now.Date.AddHours(5.10),
                    "Bus Fährt los",
                    TimeSpan.FromHours(0.4)
                );
            await addAppointment(appointment);

            appointment = newTestAppointment
                (
                    DateTime.Now.Date.AddHours(50.30),
                    "Familie besuchen",
                    TimeSpan.FromHours(1.0)
                );
            await addAppointment(appointment);

        }

        private static Appointment newTestAppointment(DateTime dt, string subject, TimeSpan ts)
        {
            Appointment appointment;
            appointment = newAppointmentPreset(dt);
            appointment.Subject = subject;
            appointment.Duration = ts;
            return appointment;
        }
        #endregion
    }
}
