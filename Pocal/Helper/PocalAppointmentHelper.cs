using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Pocal.ViewModel;
using Shared.Helper;

namespace Pocal.Helper
{
    public class PocalAppointmentHelper
    {
        // ******** *************************** *********//
        // ******** Edits exisiting Appointment *********//
        // ******** *************************** *********//


        public static async void EditAppointment(PocalAppointment pA)
        {
            var appt = await CalendarAPI.Edit(pA.Appt);

            PocalAppointment newPa = null;
            if (appt != null)
            {
                newPa = await App.ViewModel.CreatePocalAppoinment(appt);
            }
            PocalAppointmentUpdater.Update(pA, newPa);
        }

        #region NEW

        // ******** *********************** *********//
        // ******** Creates New Appointment *********//
        // ******** *********************** *********//

        public static async void AddAppointment(DateTime starttime)
        {
            var appointment = NewAppointmentPreset(starttime);
            await AddAppointment(appointment);
        }

        public static async void AddAllDayAppointment(DateTime starttime)
        {
            var appointment = NewAppointmentPreset(starttime);
            appointment.AllDay = true;
            await AddAppointment(appointment);
        }

        private static async Task AddAppointment(Appointment appointment)
        {
            var newAppointment = await CalendarAPI.Add(appointment);
            if (newAppointment != null)
            {
                var newPa = await App.ViewModel.CreatePocalAppoinment(newAppointment);

                PocalAppointmentUpdater.Update(null, newPa);
            }
        }

        private static Appointment NewAppointmentPreset(DateTime starttime)
        {
            var appointment = new Appointment
            {
                StartTime = starttime,
                Duration = TimeSpan.FromHours(1),
                Reminder = TimeSpan.FromMinutes(15)
            };
            return appointment;
        }


        public static async Task<List<Appointment>> GetReccurantAppointments(Appointment currentAppointment,
            string localId)
        {
            var appointmentStore =
                await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);

            var resultList = new List<Appointment>();
            if (currentAppointment.Recurrence != null)
            {
                var calendar =
                    await appointmentStore.GetAppointmentCalendarAsync(currentAppointment.CalendarId);


                var appointmentInstances = await
                    calendar.FindAllInstancesAsync(
                        localId,
                        DateTime.Today,
                        TimeSpan.FromDays(App.ViewModel.Days.Count),
                        CalendarAPI.GetFindOptions());

                resultList.AddRange(appointmentInstances);
            }
            else
                resultList.Add(currentAppointment);
            return resultList;
        }

        #endregion

        #region TestAppointments

        public static async void AddTestAppointments()
        {
            var appointment = NewTestAppointment
                (
                    DateTime.Now.Date.AddHours(1.50),
                    "Einkaufen",
                    TimeSpan.FromHours(2.0)
                );
            if (appointment != null)
            {
                await AddAppointment(appointment);
            }


            appointment = NewTestAppointment
                (
                    DateTime.Now.Date.AddHours(4.30),
                    "Sport",
                    TimeSpan.FromHours(3.0)
                );
            await AddAppointment(appointment);

            appointment = NewTestAppointment
                (
                    DateTime.Now.Date.AddHours(81.30),
                    "In Urlaub fahren",
                    TimeSpan.FromHours(10.0)
                );
            await AddAppointment(appointment);

            appointment = NewTestAppointment
                (
                    DateTime.Now.Date.AddHours(5.50),
                    "Jeans kaufen",
                    TimeSpan.FromHours(2.0)
                );
            await AddAppointment(appointment);

            appointment = NewTestAppointment
                (
                    DateTime.Now.Date.AddHours(5.10),
                    "Bus Fährt los",
                    TimeSpan.FromHours(0.4)
                );
            await AddAppointment(appointment);

            appointment = NewTestAppointment
                (
                    DateTime.Now.Date.AddHours(50.30),
                    "Familie besuchen",
                    TimeSpan.FromHours(1.0)
                );
            await AddAppointment(appointment);
        }

        private static Appointment NewTestAppointment(DateTime dt, string subject, TimeSpan ts)
        {
            var appointment = NewAppointmentPreset(dt);
            appointment.Subject = subject;
            appointment.Duration = ts;
            return appointment;
        }

        #endregion
    }
}