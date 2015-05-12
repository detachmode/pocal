using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;

namespace Shared.Helper
{
    public static class CalendarAPI
    {
        //MEMBER VARIABLES
        public static IReadOnlyList<AppointmentCalendar> Calendars;
        private static AppointmentStore _appointmentStore;

        public static async Task SetCalendars(bool force)
        {
            if (force)
            {
                Calendars = await _appointmentStore.FindAppointmentCalendarsAsync();
            }
            else if (Calendars == null)
                Calendars = await _appointmentStore.FindAppointmentCalendarsAsync();
        }

        private static async Task SetAppointmentStore()
        {
            if (_appointmentStore == null)
                _appointmentStore =
                    await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);
        }

        public static async Task<IReadOnlyList<Appointment>> GetAppointments(DateTime startDay, int howManyDays)
        {
            await SetAppointmentStore();
            var findOptions = GetFindOptions();

            return
                await
                    _appointmentStore.FindAppointmentsAsync(startDay.Date, TimeSpan.FromDays(howManyDays), findOptions);
        }

        public static FindAppointmentsOptions GetFindOptions()
        {
            var findOptions = new FindAppointmentsOptions();
            //findOptions.MaxCount = 100;
            findOptions.FetchProperties.Add(AppointmentProperties.Subject);
            findOptions.FetchProperties.Add(AppointmentProperties.Location);
            findOptions.FetchProperties.Add(AppointmentProperties.Details);
            findOptions.FetchProperties.Add(AppointmentProperties.StartTime);
            findOptions.FetchProperties.Add(AppointmentProperties.AllDay);
            findOptions.FetchProperties.Add(AppointmentProperties.Duration);
            return findOptions;
        }

        // ******** *********************** *********//
        // ******** Creates New Appointment *********//
        // ******** *********************** *********//


        public static async Task<Appointment> Add(Appointment appointment)
        {
            await SetAppointmentStore();
            var roamingId = await AppointmentManager.ShowEditNewAppointmentAsync(appointment);
            if (roamingId == null)
                return null;

            var localIDs = await _appointmentStore.FindLocalIdsFromRoamingIdAsync(roamingId);

            if (localIDs.Count == 0)
                return null;

            var localId = localIDs[0];

            return await _appointmentStore.GetAppointmentAsync(localId);
        }

        public static async Task<Appointment> Edit(Appointment appt)
        {
            Appointment newAppt;

            await SetAppointmentStore();
            Thread.Sleep(150);


            if (appt.OriginalStartTime == null)
            {
                await _appointmentStore.ShowAppointmentDetailsAsync(appt.LocalId);
                newAppt = await _appointmentStore.GetAppointmentAsync(appt.LocalId);
            }
            else
            {
                await _appointmentStore.ShowAppointmentDetailsAsync(appt.LocalId, appt.OriginalStartTime.Value);
                newAppt =
                    await _appointmentStore.GetAppointmentInstanceAsync(appt.LocalId, appt.OriginalStartTime.Value);
            }

            return newAppt;
        }
    }
}