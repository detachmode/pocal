using Pocal.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.ApplicationModel.Appointments;


namespace Pocal
{
    public static class CalendarAPI
    {
        //MEMBER VARIABLES
        private static IReadOnlyList<AppointmentCalendar> calendars;
        private static async Task setCalendars()
        {
            calendars = await appointmentStore.FindAppointmentCalendarsAsync();
        }

        private static AppointmentStore appointmentStore;
        private static async Task setAppointmentStore()
        {
            if (appointmentStore == null)
                appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);
        }

        private static IReadOnlyList<Appointment> appoinmentBuffer;
        private static async Task retrieveAppointments()
        {
            await setAppointmentStore();
            FindAppointmentsOptions findOptions = getFindOptions();

            appoinmentBuffer = await appointmentStore.FindAppointmentsAsync(DateTime.Now.Date, TimeSpan.FromDays(App.ViewModel.howManyDays), findOptions);

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


        //PUBLIC FUNCTIONS
        public static async void reloadPocalApptsAndDays()
        {
            await retrieveAppointments();

            if (appoinmentBuffer.Any())
            {
                await setCalendars();

                createPocalApptointments();
                createDays();

                // Aufgrund mangelnder Umsetzung von MVVM
                App.ViewModel.SingleDayViewModel.updateTappedDay();
            }
        }

        //PRIVATE FUNCTIONS
        private static void createPocalApptointments()
        {
            App.ViewModel.AllPocalAppointments.Clear();

            foreach (var appt in appoinmentBuffer)
            {
                var cal = calendars.First(c => c.LocalId == appt.CalendarId);
                var calColor = new System.Windows.Media.Color() { A = cal.DisplayColor.A, B = cal.DisplayColor.B, R = cal.DisplayColor.R, G = cal.DisplayColor.G };

                PocalAppointment pocalAppt = new PocalAppointment { Appt = appt, CalColor = new SolidColorBrush(calColor) };
                App.ViewModel.AllPocalAppointments.Add(pocalAppt);
            }
        }

        private static void createDays()
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
