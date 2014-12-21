using Pocal.Helper;
using System;
using System.Windows.Media;
using Windows.ApplicationModel.Appointments;

namespace Pocal.ViewModel
{
    public class PocalAppointment : ViewModelBase
    {
        private Appointment appt;
        public Appointment Appt
        {
            get
            {
                return appt;
            }
            set
            {

                appt = value;
                this.Subject = appt.Subject;
                this.Location = appt.Location;
                this.Details = appt.Details;
                this.StartTime = appt.StartTime;
                this.Duration = appt.Duration;
                this.AllDay = appt.AllDay;
                NotifyPropertyChanged("Appt");

            }
        }




        private SolidColorBrush calColor;
        public SolidColorBrush CalColor
        {
            get
            {
                return calColor;
            }
            set
            {
                calColor = value;
                NotifyPropertyChanged("CalColor");
            }

        }

        private string subject;
        public string Subject
        {
            get
            {
                return subject;
            }
            private set
            {
                if (value != subject)
                {
                    subject = value;
                    NotifyPropertyChanged("Subject");
                }
            }
        }


        private string location;
        public string Location
        {
            get
            {
                return location;
            }
            private set
            {
                if (value != location)
                {
                    location = value;
                    NotifyPropertyChanged("Location");
                }
            }
        }

        private string details;
        public string Details
        {
            get
            {
                return details;
            }
            private set
            {
                if (value != details)
                {
                    details = value;
                    NotifyPropertyChanged("Details");
                }
            }
        }

        private DateTimeOffset startTime;
        public DateTimeOffset StartTime
        {
            get
            {
                return startTime;
            }
            private set
            {
                if (value != startTime)
                {
                    startTime = value;
                    NotifyPropertyChanged("StartTime");
                }
            }
        }


        private TimeSpan duration;
        public TimeSpan Duration
        {
            get
            {
                return duration;
            }
            private set
            {
                if (value != duration)
                {
                    duration = value;
                    NotifyPropertyChanged("Duration");
                }
            }
        }

        private bool allDay;
        public bool AllDay
        {
            get
            {
                return allDay;
            }
            //todo make private again
            set
            {
                if (value != allDay)
                {
                    allDay = value;
                    NotifyPropertyChanged("AllDay");
                }
            }
        }



        private int _column;
        public int Column
        {
            get
            {
                return _column;
            }
            //todo make private again
            set
            {
                if (value != _column)
                {
                    _column = value;
                    NotifyPropertyChanged("Column");
                }
            }
        }


        private int _maxConflicts;
        public int MaxConflicts
        {
            get
            {
                return _maxConflicts;
            }
            //todo make private again
            set
            {
                if (value != _maxConflicts)
                {
                    _maxConflicts = value;
                    NotifyPropertyChanged("MaxConflicts");
                }
            }
        }

        public bool isInTimeFrameOfDate(DateTime date)
        {
            return TimeFrameChecker.isInTimeFrameOfDay(this.Appt, date);
        }

        public bool isInTimeFrame(DateTime start, DateTime end)
        {
            return TimeFrameChecker.isInTimeFrame(this.Appt, start, end);

        }

        private bool isInTimeFrameOFStartEnd(DateTimeOffset start, DateTimeOffset end)
        {
            return TimeFrameChecker.isInTimeFrameOFStartEnd(this.Appt, start, end);
        }

        public bool isInTimeFrame_IgnoreAllDays(DateTime start, DateTime end)
        {
            if (this.AllDay)
                return false;

            return isInTimeFrameOFStartEnd(start, end);

        }




    }
}
