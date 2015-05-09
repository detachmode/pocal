using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using System.Threading;

namespace Shared.Helper
{
    public static class CalendarAPI
    {
        //MEMBER VARIABLES
        public static IReadOnlyList<AppointmentCalendar> calendars;
        public static async Task SetCalendars(bool force)
        {
            if (force)
            {
                calendars = await appointmentStore.FindAppointmentCalendarsAsync();
            }
            else
                if (calendars == null)
                    calendars = await appointmentStore.FindAppointmentCalendarsAsync();
        }

        private static AppointmentStore appointmentStore;
        private static async Task setAppointmentStore()
        {
            if (appointmentStore == null)
                appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);
        }



        public static async Task<IReadOnlyList<Appointment>> GetAppointments(DateTime startDay, int howManyDays)
        {
            await setAppointmentStore();
            FindAppointmentsOptions findOptions = GetFindOptions();

            return await appointmentStore.FindAppointmentsAsync(startDay.Date, TimeSpan.FromDays(howManyDays), findOptions);

        }

        public static FindAppointmentsOptions GetFindOptions()
        {
            FindAppointmentsOptions findOptions = new FindAppointmentsOptions();
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
            await setAppointmentStore();
            String roamingId = await AppointmentManager.ShowEditNewAppointmentAsync(appointment);
            if (roamingId == null)
                return null;

            IReadOnlyList<string> localIDs = await appointmentStore.FindLocalIdsFromRoamingIdAsync(roamingId);

            if (localIDs.Count == 0)
                return null;

            string localID = localIDs[0];

            return await appointmentStore.GetAppointmentAsync(localID);

        }



        public static async Task<Appointment> Edit(Appointment appt)
        {
            Appointment newAppt;

            await setAppointmentStore();
            Thread.Sleep(150);

           
            if (appt.OriginalStartTime == null)
            {
                await appointmentStore.ShowAppointmentDetailsAsync(appt.LocalId);
                newAppt = await appointmentStore.GetAppointmentAsync(appt.LocalId);
            }
            else
            {
                await appointmentStore.ShowAppointmentDetailsAsync(appt.LocalId, appt.OriginalStartTime.Value);
                newAppt = await appointmentStore.GetAppointmentInstanceAsync(appt.LocalId, appt.OriginalStartTime.Value);
            }

            return newAppt;


        }

    }

}
