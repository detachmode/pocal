
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;


namespace ScheduledTaskAgent1
{
    public static class CalendarAPI
    {
        //MEMBER VARIABLES
        public static IReadOnlyList<AppointmentCalendar> calendars;
        public static async Task setCalendars()
        {
            if (calendars == null)
                calendars = await appointmentStore.FindAppointmentCalendarsAsync();
        }

        public static AppointmentStore appointmentStore;
        public static async Task setAppointmentStore()
        {
            if (appointmentStore == null)
                appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);
        }

        #region GET DAYS


        public static async Task<IReadOnlyList<Appointment>> getAppointments(DateTime startDay, int howManyDays)
        {
            await setAppointmentStore();
            FindAppointmentsOptions findOptions = getFindOptions();

            return await appointmentStore.FindAppointmentsAsync(startDay.Date, TimeSpan.FromDays(howManyDays), findOptions);

        }



        private static FindAppointmentsOptions getFindOptions()
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
        #endregion




    }
}
