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
            // soll Aufflackern des Termins verhindern, wenn keine Änderungen gemacht wurden.
            if (oldPA != null && newPA != null)
                if (arePocaAppointmentsEqual(oldPA, newPA))
                    return;

            updateIfSingle(oldPA, newPA);
            updateIfRecurrent(oldPA, newPA);
        }

        private static bool arePocaAppointmentsEqual(PocalAppointment a, PocalAppointment b)
        {

            if (a.AllDay != b.AllDay) return false;
            if (a.Subject != b.Subject) return false;  
            if (a.Location != b.Location) return false;
            if (a.StartTime != b.StartTime) return false;
            if (a.Duration != b.Duration) return false;
            if (a.Details != b.Details) return false;
            if (a.Appt.Recurrence != b.Appt.Recurrence) return false;
            return true;
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

                for (int i = day.PocalApptsOfDay.Count - 1; i >= 0; i--)
                {
                    if (day.PocalApptsOfDay[i].Appt.LocalId == appt.LocalId)
                        day.PocalApptsOfDay.RemoveAt(i);
                }
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


        private static void add(PocalAppointment pA)
        {
            setResponsibleDays(pA);
            foreach (var day in responsibleDaysOfPA)
            {
                InsertInto_PocalApptsOfDay(day, pA);
            }
        }

        private static void InsertInto_PocalApptsOfDay(Day day, PocalAppointment pA)
        {
            PocalAppointment nextDay = day.PocalApptsOfDay.FirstOrDefault(x => x.Appt.StartTime > pA.Appt.StartTime);
            if (nextDay != null)
            {
                int index = day.PocalApptsOfDay.IndexOf(nextDay);
                day.PocalApptsOfDay.Insert(index, pA);
            }
            else
                day.PocalApptsOfDay.Insert(day.PocalApptsOfDay.Count(), pA);


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
