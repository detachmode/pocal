using Pocal.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using Pocal.Helper;
using Windows.Foundation;

namespace Pocal
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

        #region GET 30 DAYS

        // ******** **************************************** *********//
        // ******** Get the Appointments of the next 30 Days *********//
        // ******** **************************************** *********//

        public static async Task<IReadOnlyList<Appointment>> getAppointments()
        {
            await setAppointmentStore();
            FindAppointmentsOptions findOptions = getFindOptions();

            return await appointmentStore.FindAppointmentsAsync(DateTime.Now.Date, TimeSpan.FromDays(App.ViewModel.howManyDays), findOptions);

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



        #region NEW
        // ******** *********************** *********//
        // ******** Creates New Appointment *********//
        // ******** *********************** *********//

        public static async void addAppointment(DateTime starttime)
        {
            await setAppointmentStore();

            var appointment = newAppointmentPreset(starttime);

            String roamingId = await AppointmentManager.ShowEditNewAppointmentAsync(appointment);
            if (roamingId == null)
                return;

            IReadOnlyList<string> localIDs = await appointmentStore.FindLocalIdsFromRoamingIdAsync(roamingId);

            if (localIDs.Count == 0)
                return;

            string localID = localIDs[0];

            Appointment newAppointment = await appointmentStore.GetAppointmentAsync(localID);
            PocalAppointment newPa = App.ViewModel.createPocalAppoinment(newAppointment);

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



        private static async Task<List<Appointment>> getReccurantAppointments(Appointment currentAppointment, string localID)
        {
            AppointmentStore appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);

            List<Appointment> resultList = new List<Appointment>();
            if (currentAppointment.Recurrence != null)
            {
                AppointmentCalendar calendar =
                    await appointmentStore.GetAppointmentCalendarAsync(currentAppointment.CalendarId);

                FindAppointmentsOptions options = new FindAppointmentsOptions();


                IReadOnlyList<Appointment> appointmentInstances = await
                    calendar.FindAllInstancesAsync(
                        localID,
                        DateTime.Today,
                        TimeSpan.FromDays(30),
                        options);

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



        #region EDIT
        // ******** *************************** *********//
        // ******** Edits exisiting Appointment *********//
        // ******** *************************** *********//


        public static async void editAppointment(PocalAppointment pA)
        {
            await setAppointmentStore();

            Appointment newAppt;

            if (pA.Appt.OriginalStartTime == null)
            {
                await appointmentStore.ShowAppointmentDetailsAsync(pA.Appt.LocalId);
                newAppt = await appointmentStore.GetAppointmentAsync(pA.Appt.LocalId);
            }
            else
            {
                await appointmentStore.ShowAppointmentDetailsAsync(pA.Appt.LocalId, pA.Appt.OriginalStartTime.Value);
                newAppt = await appointmentStore.GetAppointmentInstanceAsync(pA.Appt.LocalId, pA.Appt.OriginalStartTime.Value);
            }

            PocalAppointment newPA = null;
            if (newAppt != null)
            {
                newPA = App.ViewModel.createPocalAppoinment(newAppt);
            }
            PocalAppointmentUpdater.Update(pA, newPA);
        }



        #endregion


    }
}
