﻿using Pocal.ViewModel;
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

namespace Pocal
{
    public static class CalendarAPI
    {
        //MEMBER VARIABLES
        public static IReadOnlyList<AppointmentCalendar> calendars;
        public static async Task setCalendars()
        {
            calendars = await appointmentStore.FindAppointmentCalendarsAsync();
        }

        public static AppointmentStore appointmentStore;
        public static async Task setAppointmentStore()
        {
            if (appointmentStore == null)
                appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);
        }

        #region Get Appointments
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
            findOptions.FetchProperties.Add(AppointmentProperties.StartTime);
            findOptions.FetchProperties.Add(AppointmentProperties.AllDay);
            findOptions.FetchProperties.Add(AppointmentProperties.Duration);
            return findOptions;
        }
        #endregion


        #region Add Appoinment
        public static async void addAppointment_old(DateTime starttime)
        {
            await setAppointmentStore();

            var appointment = new Windows.ApplicationModel.Appointments.Appointment();

            appointment.StartTime = starttime;
            appointment.Duration = TimeSpan.FromHours(1);
            appointment.Reminder = TimeSpan.FromMinutes(15);

            string appointmentId = await AppointmentManager.ShowEditNewAppointmentAsync(appointment);
            var x = 1;

            var appt = await appointmentStore.GetAppointmentAsync(appointmentId);
            

            // Aufgrund mangelnder Umsetzung von MVVM
            App.ViewModel.ReloadPocalApptsAndDays();

        }

        public static async void addAppointment(DateTime starttime)
        {
            await setAppointmentStore();
            
            //create Appointment
            var appointment = new Windows.ApplicationModel.Appointments.Appointment();
            appointment.StartTime = starttime;
            appointment.Duration = TimeSpan.FromHours(1);

           
            // Add Appoinment API
            string localAppointmentId = await AppointmentManager.ShowEditNewAppointmentAsync(appointment);
            var x = 1;
            // get Added Appoinment
            var currentAppointment = await appointmentStore.GetAppointmentAsync(localAppointmentId);
            Debug.WriteLine("Debugger");

            x = 1;

        }


        
        #endregion

        // Bedarf einiges an Verbesserungen mit MVVM
        #region Edit Appointments


        public static async void editAppointment(PocalAppointment pocalappt)
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


        private static void updatePocalAppointment(Appointment appt, PocalAppointment pocalappt)
        {

            removeIfApptIsNull(appt, pocalappt);

            if (isCompleteRefreshNeeded(appt))
            {
                System.Diagnostics.Debug.WriteLine("CompleteRefresh");
                App.ViewModel.ReloadPocalApptsAndDays();
            }
            else
            {
                int indexInTappedDayAppts = App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay.IndexOf(pocalappt);
                App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay[indexInTappedDayAppts].Appt = appt;
            }
        }

        private static void removeIfApptIsNull(Appointment appt, PocalAppointment pocalappt)
        {
            if (appt == null)
                App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay.Remove(pocalappt);
        }

        private static bool isCompleteRefreshNeeded(Appointment appt)
        {

            // Appointment wurde gelöscht -> auch refresh
            if (appt == null)
                return true;

            // Bei sich wiederholenden AllPocalAppointments wird immer refresht
            if (appt.Recurrence != null)
                return true;

            // nur refresh wenn der Termin sich innerhalb des tappedDays befindet
            return !(isOnlyApptOfTappedDay(appt));

        }

        private static bool isOnlyApptOfTappedDay(Appointment appt)
        {
            var day = App.ViewModel.SingleDayViewModel.TappedDay;
            var endTime = (appt.StartTime + appt.Duration);

            return ((appt.StartTime.Date == day.DT.Date) && (endTime.Date == day.DT.Date));
        }
        #endregion


    }
}
