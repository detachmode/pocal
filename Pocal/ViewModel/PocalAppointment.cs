using Pocal.Helper;
using System;
using System.Windows.Media;
using Windows.ApplicationModel.Appointments;

namespace Pocal.ViewModel
{
    public class PocalAppointment : ViewModelBase
    {
        private Appointment _appt;
        public Appointment Appt
        {
            get
            {
                return _appt;
            }
            set
            {

                _appt = value;
                Subject = _appt.Subject;
                Location = _appt.Location;
                Details = _appt.Details;
                StartTime = _appt.StartTime;
                Duration = _appt.Duration;
                AllDay = _appt.AllDay;
                NotifyPropertyChanged("Appt");

            }
        }




        private SolidColorBrush _calColor;
        public SolidColorBrush CalColor
        {
            get
            {
                return _calColor;
            }
            set
            {
                _calColor = value;
                NotifyPropertyChanged("CalColor");
            }

        }

        private string _subject;
        public string Subject
        {
            get
            {
                return _subject;
            }
            private set
            {
                if (value != _subject)
                {
                    _subject = value;
                    NotifyPropertyChanged("Subject");
                }
            }
        }


        private string _location;
        public string Location
        {
            get
            {
                return _location;
            }
            private set
            {
                if (value != _location)
                {
                    _location = value;
                    NotifyPropertyChanged("Location");
                }
            }
        }

        private string _details;
        public string Details
        {
            get
            {
                return _details;
            }
            private set
            {
                if (value != _details)
                {
                    _details = value;
                    NotifyPropertyChanged("Details");
                }
            }
        }

        private DateTimeOffset _startTime;
        public DateTimeOffset StartTime
        {
            get
            {
                return _startTime;
            }
            private set
            {
                if (value != _startTime)
                {
                    _startTime = value;
                    NotifyPropertyChanged("StartTime");
                }
            }
        }


        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get
            {
                return _duration;
            }
            private set
            {
                if (value == _duration) return;
                _duration = value;
                NotifyPropertyChanged("Duration");
            }
        }

        private bool _allDay;
        public bool AllDay
        {
            get
            {
                return _allDay;
            }

            set
            {
                if (value == _allDay) return;
                _allDay = value;
                NotifyPropertyChanged("AllDay");
            }
        }



        private int _column;
        public int Column
        {
            get
            {
                return _column;
            }
            set
            {
                if (value == _column) return;
                _column = value;
                NotifyPropertyChanged("Column");
            }
        }


        private int _maxConflicts;
        public int MaxConflicts
        {
            get
            {
                return _maxConflicts;
            }
            set
            {
                if (value == _maxConflicts) return;
                _maxConflicts = value;
                NotifyPropertyChanged("MaxConflicts");
            }
        }

        public bool IsInTimeFrameOfDate(DateTime date)
        {
            return TimeFrameChecker.IsInTimeFrameOfDay(Appt, date);
        }

        public bool IsInTimeFrame(DateTime start, DateTime end)
        {
            return TimeFrameChecker.IsInTimeFrame(Appt, start, end);

        }

        private bool IsInTimeFrameOfStartEnd(DateTimeOffset start, DateTimeOffset end)
        {
            return TimeFrameChecker.IsInTimeFrameOfStartEnd(Appt, start, end);
        }

        public bool isInTimeFrame_IgnoreAllDays(DateTime start, DateTime end)
        {
            if (AllDay)
                return false;

            return IsInTimeFrameOfStartEnd(start, end);

        }




    }
}
