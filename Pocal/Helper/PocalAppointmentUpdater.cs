using Pocal.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;

namespace Pocal.Helper
{
    public static class PocalAppointmentUpdater
    {

        private static List<Day> responsibleDaysOfPA;



        public static void Update(PocalAppointment oldPA, PocalAppointment newPA)
        {          
            updateIfSingle(oldPA,newPA);
            updateIfRecurrent(oldPA, newPA);
        }

        private static void updateIfRecurrent(PocalAppointment oldPA, PocalAppointment newPA)
        {
            if (oldPA != null && oldPA.Appt.Recurrence != null)
            {
                removeRecurrent(oldPA.Appt);
                addRecurrent(oldPA.Appt);
            }
            else if (newPA != null && newPA.Appt.Recurrence != null)
                addRecurrent(newPA.Appt);
        }

        private static void updateIfSingle(PocalAppointment oldPA, PocalAppointment newPA)
        {
            if (oldPA != null && oldPA.Appt.Recurrence == null)
                removeAllWith(oldPA.Appt.LocalId);

            if (newPA != null && newPA.Appt.Recurrence == null)
                add(newPA);
        }




        private static void removeRecurrent(Appointment appt)
        {
            foreach (var day in App.ViewModel.Days)
            {
                // ?? is .First enough, or will there be more PAs with the same Appt.LocalID?
                PocalAppointment pa = day.PocalApptsOfDay.FirstOrDefault(x => x.Appt.LocalId == appt.LocalId);

                // Remove pa, if LocalID was found in an PocalAppointment in this Day
                if (pa != null)
                    day.PocalApptsOfDay.Remove(pa);


            }
        }
        private static void removeAllWith(string localId)
        {
            // Bruteforce - All Days in Cache will be searched for the PocalAppointment with the Appt.LocalID
            foreach (var day in App.ViewModel.Days)
            {
                // ?? is .First enough, or will there be more PAs with the same Appt.LocalID?
                PocalAppointment pa = day.PocalApptsOfDay.FirstOrDefault(x => x.Appt.LocalId == localId);

                // Remove pa, if LocalID was found in an PocalAppointment in this Day
                if (pa != null)
                    day.PocalApptsOfDay.Remove(pa);


            }
        }


        private static async void addRecurrent(Appointment appt)
        {
            List<Appointment> appts = await getReccurantAppointments(appt);
            //List<Appointment> appts = await getAppointmentsWithLocalId(appt.LocalId);

            foreach (var a in appts)
            {
                PocalAppointment pa = App.ViewModel.createPocalAppoinment(a);
                add(pa);
            }


        }

        private static Task<List<Appointment>> getAppointmentsWithLocalId(string p)
        {
            throw new NotImplementedException();
        }

        private static void add(PocalAppointment pA)
        {
            setResponsibleDays(pA);
            foreach (var day in responsibleDaysOfPA)
            {
                day.PocalApptsOfDay.Add(pA);

                day.PocalApptsOfDay = App.ViewModel.sortAppointments(day.PocalApptsOfDay);
            }
        }

        private static void setResponsibleDays(PocalAppointment pA)
        {
            responsibleDaysOfPA = new List<Day>();
            foreach (Day day in App.ViewModel.Days)
            {
                if (isInTimeFrame(pA, day))
                {
                    responsibleDaysOfPA.Add(day);
                }
            }
        }

        private static bool isInTimeFrame(PocalAppointment pA, Day day)
        {
            DateTimeOffset starttime = pA.StartTime;
            DateTimeOffset endtime = starttime + pA.Duration;

            bool endsBeforeThisDay = (endtime.Date < day.DT.Date);
            bool beginsAfterThisDay = (starttime.Date > day.DT.Date); ;

            bool isNotInTimeFrame = (endsBeforeThisDay || beginsAfterThisDay);

            return !isNotInTimeFrame;

        }



        private static async Task<List<Appointment>> getReccurantAppointments(Appointment appt)
        {
            //await CalendarAPI.setAppointmentStore();
            List<Appointment> resultList = new List<Appointment>();

            AppointmentCalendar calendar = await CalendarAPI.appointmentStore.GetAppointmentCalendarAsync(appt.CalendarId);

            FindAppointmentsOptions options = new FindAppointmentsOptions();


            IReadOnlyList<Appointment> appointmentInstances = await
                calendar.FindAllInstancesAsync(
                    appt.LocalId,
                    DateTime.Today,
                    TimeSpan.FromDays(App.ViewModel.howManyDays),
                    options);

            foreach (var a in appointmentInstances)
                resultList.Add(a);

            return resultList;
        }

    }
}
