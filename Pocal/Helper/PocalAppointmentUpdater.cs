using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Pocal.ViewModel;

namespace Pocal.Helper
{
    public static class PocalAppointmentUpdater
    {
        private static List<Day> _responsibleDaysOfPa;

        public static async void Update(PocalAppointment oldPa, PocalAppointment newPa)
        {
            // soll Aufflackern des Termins verhindern, wenn keine Änderungen gemacht wurden.
            if (oldPa != null && newPa != null)
                if (ArePocaAppointmentsEqual(oldPa, newPa))
                    return;

            UpdateIfSingle(oldPa, newPa);
            await UpdateIfRecurrent(oldPa, newPa);

            App.ViewModel.ConflictManager.SolveConflicts();

            ViewSwitcher.Mainpage.SingleDayViewer.Update_PocalAppointment(oldPa, newPa);
        }

        private static bool ArePocaAppointmentsEqual(PocalAppointment a, PocalAppointment b)
        {
            if (a.AllDay != b.AllDay) return false;
            if (a.Subject != b.Subject) return false;
            if (a.Location != b.Location) return false;
            if (a.StartTime != b.StartTime) return false;
            if (a.Duration != b.Duration) return false;
            if (a.Details != b.Details) return false;
            if (!IsRecurrenceEqual(a, b)) return false;
            return true;
        }

        private static bool IsRecurrenceEqual(PocalAppointment a, PocalAppointment b)
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

        private static void UpdateIfSingle(PocalAppointment oldPa, PocalAppointment newPa)
        {
            if (oldPa != null && oldPa.Appt.Recurrence == null)
                RemoveAllWith(oldPa.Appt.LocalId);

            if (newPa != null && newPa.Appt.Recurrence == null)
                Add(newPa);

        }

        private static async Task UpdateIfRecurrent(PocalAppointment oldPa, PocalAppointment newPa)
        {
            if (oldPa != null && oldPa.Appt.Recurrence != null)
            {
                // Todo fixme
                RemoveRecurrent(oldPa.Appt);

                // für den fall, dass Nur ein Termin entfernt wurde, müssen alle anderen alten Recurrenztermine wieder neu geaddet werden
                if (newPa == null)
                {
                    await AddRecurrent(oldPa.Appt); 
                }
            }

            if (newPa != null && newPa.Appt.Recurrence != null)
            {
                await AddRecurrent(newPa.Appt);
            }
        }

        private static void RemoveRecurrent(Appointment appt)
        {
            foreach (var day in App.ViewModel.Days)
            {
                for (var i = day.PocalApptsOfDay.Count - 1; i >= 0; i--)
                {
                    if (day.PocalApptsOfDay[i].Appt.LocalId == appt.LocalId)
                        day.PocalApptsOfDay.RemoveAt(i);
                }
            }
        }

        private static void RemoveAllWith(string localId)
        {
            // Bruteforce - All Days in Cache will be searched for the PocalAppointment with the Appt.LocalID
            foreach (var day in App.ViewModel.Days)
            {
                var pa = day.PocalApptsOfDay.FirstOrDefault(x => x.Appt.LocalId == localId);

                // Remove pa, if LocalID was found in an PocalAppointment in this Day
                if (pa != null)
                    day.PocalApptsOfDay.Remove(pa);
            }
        }

        private static void Add(PocalAppointment pA)
        {
            SetResponsibleDays(pA);
            foreach (var day in _responsibleDaysOfPa)
            {
                InsertInto_PocalApptsOfDay(day, pA);
            }
        }

        private static async Task AddRecurrent(Appointment appt)
        {
            var appts = await PocalAppointmentHelper.GetReccurantAppointments(appt, appt.LocalId);

            foreach (var a in appts)
            {
                var pa = await App.ViewModel.CreatePocalAppoinment(a);
                Add(pa);
            }
        }

        private static void InsertInto_PocalApptsOfDay(Day day, PocalAppointment pA)
        {
            var nextDay = day.PocalApptsOfDay.FirstOrDefault(x => x.Appt.StartTime > pA.Appt.StartTime);
            if (nextDay != null)
            {
                var index = day.PocalApptsOfDay.IndexOf(nextDay);
                day.PocalApptsOfDay.Insert(index, pA);
            }
            else
                day.PocalApptsOfDay.Insert(day.PocalApptsOfDay.Count(), pA);
        }

        private static void SetResponsibleDays(PocalAppointment pA)
        {
            _responsibleDaysOfPa = new List<Day>();
            foreach (var day in App.ViewModel.Days.Where(day => pA.IsInTimeFrameOfDate(day.Dt)))
            {
                _responsibleDaysOfPa.Add(day);
            }
        }
    }
}