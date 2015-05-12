using Pocal.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;

namespace Pocal.Helper
{
    public static class PocalAppointmentUpdater
    {

        private static List<Day> responsibleDaysOfPA;



        public static async void Update(PocalAppointment oldPA, PocalAppointment newPA)
        {
            // soll Aufflackern des Termins verhindern, wenn keine Änderungen gemacht wurden.
            if (oldPA != null && newPA != null)
                if (arePocaAppointmentsEqual(oldPA, newPA))
                    return;

            updateIfSingle(oldPA, newPA);
            await updateIfRecurrent(oldPA, newPA);

            App.ViewModel.ConflictManager.solveConflicts();

            ViewSwitcher.mainpage.SingleDayViewer.Update_PocalAppointment(oldPA,newPA);

            
        }

        private static bool arePocaAppointmentsEqual(PocalAppointment a, PocalAppointment b)
        {

            if (a.AllDay != b.AllDay) return false;
            if (a.Subject != b.Subject) return false;  
            if (a.Location != b.Location) return false;
            if (a.StartTime != b.StartTime) return false;
            if (a.Duration != b.Duration) return false;
            if (a.Details != b.Details) return false;
            if (!isRecurrenceEqual(a, b)) return false;
            return true;
        }

        private static bool isRecurrenceEqual(PocalAppointment a, PocalAppointment b)
        {
            if (a.Appt.Recurrence == null && b.Appt.Recurrence == null)
                return true;

            // Bei Änderung von Recurrence zu nicht Recurrenz, oder anders herum.
            if (a.Appt.Recurrence == null || b.Appt.Recurrence == null)
                return false;

            if (a.Appt.Recurrence.Day != b.Appt.Recurrence.Day) return false;
            if (a.Appt.Recurrence.DaysOfWeek != b.Appt.Recurrence.DaysOfWeek) return false;
            if (a.Appt.Recurrence.Interval != b.Appt.Recurrence.Interval) return false;
            if (a.Appt.Recurrence.Month != b.Appt.Recurrence.Month) return false;
            if (a.Appt.Recurrence.RecurrenceType != b.Appt.Recurrence.RecurrenceType) return false;

            return true;
        }

        private static void updateIfSingle(PocalAppointment oldPA, PocalAppointment newPA)
        {
            if (oldPA != null && oldPA.Appt.Recurrence == null)
                removeAllWith(oldPA.Appt.LocalId);

            if (newPA != null && newPA.Appt.Recurrence == null)
                add(newPA);
        }


        private static async Task updateIfRecurrent(PocalAppointment oldPA, PocalAppointment newPA)
        {
            
            if (oldPA != null && oldPA.Appt.Recurrence != null)
                removeRecurrent(oldPA.Appt);           
            
            if (newPA != null && newPA.Appt.Recurrence != null)
                await addRecurrent(newPA.Appt);
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
                PocalAppointment pa = day.PocalApptsOfDay.FirstOrDefault(x => x.Appt.LocalId == localId);

                // Remove pa, if LocalID was found in an PocalAppointment in this Day
                if (pa != null)
                    day.PocalApptsOfDay.Remove(pa);

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


        private static async Task addRecurrent(Appointment appt)
        {
            List<Appointment> appts = await PocalAppointmentHelper.GetReccurantAppointments(appt, appt.LocalId);

            foreach (var a in appts)
            {
                PocalAppointment pa = await App.ViewModel.CreatePocalAppoinment(a);
                add(pa);
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
                if (pA.IsInTimeFrameOfDate(day.Dt))
                {
                    responsibleDaysOfPA.Add(day);
                }
            }
        }

    }
}
