using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Windows.ApplicationModel.Appointments;
using Pocal.Helper;
using ScreenSizeSupport;
using Shared.Helper;

namespace Pocal.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public enum Modi
        {
            AgendaView,
            OverView,
            OverViewSdv,
            AgendaViewSdv,
            MonthView,
            SearchView
        };

        private Day _dayAtPointer;
        private ObservableCollection<Day> _days;
        private int _loadEnoughCounter;
        private double _newestGoToDateStamp = DateTime.Now.Ticks;
        private string _time;
        public Modi InModus = Modi.AgendaView;
        public bool IsCurrentlyLoading;
        public Modi ModusBefore;

        public MainViewModel()
        {
           
            Days = new ObservableCollection<Day>();
            SingleDayViewModel = new SingleDayViewVM();
            ConflictManager = new ConflictManager();
            SettingsViewModel = new SettingsViewModel();

            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000); // TODO performance
            dispatcherTimer.Start();

            #region DESIGN TIME DATA

            if (!DesignerProperties.IsInDesignTool) return;
            App.DisplayInformationEmulator =
                Application.Current.Resources["DisplayInformationEmulator"] as DisplayInformationEmulator;


            createDesignData();

            #endregion
        }

        private void createDesignData()
        {
        	//CREATE DESIGN TIME DATA HERE
            var start = new DateTime(2014, 11, 07);
            var dt = start - start.TimeOfDay;

            var dt0 = dt;

            var dt2 = dt0.AddDays(2);
            var dt3 = dt0.AddDays(3);
            var dt4 = dt0.AddDays(4);
            var ts = new TimeSpan(1, 30, 0);
            var ts2 = new TimeSpan(2, 0, 0);
            var ts3 = new TimeSpan(3, 0, 0);

            var designDataDayappts = new ObservableCollection<Appointment>();
            SearchResults = new ObservableCollection<PocalAppointment>();

            var calColorOrange = new SolidColorBrush(Colors.Orange);
            var calColorRed = new SolidColorBrush(Colors.Red);


            //var appt = new Appointment
            //{
            //    Subject = "Gameplay Programming",
            //    StartTime = dt2.AddHours(8.5),
            //    Duration = ts3
            //};
            //var pa = await CreatePocalAppoinment(appt);

            //SearchResults.Add(pa);

            designDataDayappts.Add(new Appointment
            {
                Subject = "IT Security",
                StartTime = dt2.AddHours(11.75),
                Duration = ts
            });


            designDataDayappts.Add(new Appointment
            {
                Subject = "Gameplay Programming",
                StartTime = dt2.AddHours(8.5),
                Duration = ts3
            });
            designDataDayappts.Add(new Appointment
            {
                Subject = "IT Security",
                StartTime = dt2.AddHours(11.75),
                Duration = ts
            });


            designDataDayappts.Add(new Appointment
            {
                Subject = "Structered Data and Application",
                StartTime = dt3.AddHours(0),
                AllDay = false,
                Location = "HdM Raum 011",
                Details =
                    "EEine Notiz wie keine Zweite.Und auch keine Dritte ! Oder eine Vierte. Oder eine Fünfte Eine Notiz wie keine Zweite.Und auch keine Dritte ! Oder eine Vierte. Oder eine Fünfte ine Notiz wie keine Zweite.Und auch keine Dritte ! Oder eine Vierte. Oder eine Fünfte.Eine Notiz wie keine Zweite.Und auch keine Dritte ! Oder eine Vierte. Oder eine Fünfte",
                Duration = ts2
            });
            //DesignDataDayappts.Add(new Appointment { Subject = "Artificial Intelligence for Games", StartTime = dt3.AddHours(14), Duration = ts3 });
            designDataDayappts.Add(new Appointment
            {
                Subject = "Exercises Structered Data and Application",
                StartTime = dt4.AddHours(1.5),
                AllDay = false,
                Duration = ts3
            });
            designDataDayappts.Add(new Appointment
            {
                Subject = "IT Security",
                StartTime = dt4.AddHours(2.75),
                Duration = ts
            });
            designDataDayappts.Add(new Appointment
            {
                Subject = "BWL für Informatiker",
                StartTime = dt4.AddHours(16.00),
                Duration = ts
            });


            designDataDayappts.Add(new Appointment
            {
                Subject = "Geburtstag von Peter Pan",
                StartTime = dt.AddHours(0),
                AllDay = true
            });
            designDataDayappts.Add(new Appointment
            {
                Subject = "Geburtstag von Bob Marley",
                StartTime = dt.AddHours(0),
                AllDay = true
            });
            designDataDayappts.Add(new Appointment
            {
                Subject = "Essen",
                StartTime = dt.AddHours(9),
                Duration = ts,
                Location = "Stuttgart"
            });
            designDataDayappts.Add(new Appointment
            {
                Subject = "Einkaufen",
                StartTime = dt.AddHours(11.3),
                Duration = ts
            });
            designDataDayappts.Add(new Appointment
            {
                Subject = "Mom Anrufen",
                StartTime = dt.AddHours(14.5),
                Duration = ts
            });

            var designDataDay2Pocalappts = new ObservableCollection<PocalAppointment>
            {
                new PocalAppointment {Appt = designDataDayappts[6], CalColor = calColorOrange},
                new PocalAppointment {Appt = designDataDayappts[7], CalColor = calColorOrange},
                new PocalAppointment {Appt = designDataDayappts[8], CalColor = calColorOrange},
                new PocalAppointment {Appt = designDataDayappts[0], CalColor = calColorRed},
                new PocalAppointment {Appt = designDataDayappts[9], CalColor = calColorOrange},
                new PocalAppointment {Appt = designDataDayappts[10], CalColor = calColorOrange},
                new PocalAppointment {Appt = designDataDayappts[1], CalColor = calColorRed}
            };


            var designDataDay3Pocalappts = new ObservableCollection<PocalAppointment>
            {
                new PocalAppointment {Appt = designDataDayappts[6], CalColor = calColorRed, AllDay = true},
                new PocalAppointment {Appt = designDataDayappts[2], CalColor = calColorRed, AllDay = false},
                new PocalAppointment {Appt = designDataDayappts[3], CalColor = calColorRed, AllDay = false}
            };


            var designDataDay4Pocalappts = new ObservableCollection<PocalAppointment>
            {
                new PocalAppointment {Appt = designDataDayappts[4], CalColor = calColorRed},
                new PocalAppointment {Appt = designDataDayappts[5], CalColor = calColorRed},
                new PocalAppointment {Appt = designDataDayappts[6], CalColor = calColorRed}
            };


            Days.Add(new Day {Dt = dt2, PocalApptsOfDay = designDataDay2Pocalappts, Sunday = false});
            Days.Add(new Day {Dt = dt3, PocalApptsOfDay = designDataDay3Pocalappts, Sunday = false});
            Days.Add(new Day {Dt = dt4, PocalApptsOfDay = designDataDay4Pocalappts, Sunday = false});


            SingleDayViewModel.TappedDay = Days[0];
            DayAtPointer = new Day {Dt = dt.AddHours(24)};
        }

        public ObservableCollection<Day> Days
        {
            get { return _days; }
            set
            {
                if (value != _days)
                {
                    _days = value;
                    NotifyPropertyChanged("Days");
                }
            }
        }

        public DateTime LastCachedDate = DateTime.Today;
        public IReadOnlyList<Appointment> CachedAppointmentsForSearch;

        public ObservableCollection<PocalAppointment> SearchResults
        {
            get { return _searchResults; }
            set
            {
                if (value != _searchResults)
                {
                    _searchResults = value;
                    NotifyPropertyChanged("SearchResults");
                }
            }
        }

        

        public SingleDayViewVM SingleDayViewModel { get; private set; }
        public ConflictManager ConflictManager { get; private set; }
        public SettingsViewModel SettingsViewModel { get; private set; }

        public Day DayAtPointer
        {
            get { return _dayAtPointer; }
            set
            {
                if (value != _dayAtPointer)
                {
                    _dayAtPointer = value;
                    NotifyPropertyChanged("DayAtPointer");
                }
            }
        }

        public string Time
        {
            get { return _time; }
            private set
            {
                if (value == _time) return;
                _time = value;
                NotifyPropertyChanged("Time");
            }
        }


        internal bool IsInOverviewModus()
        {
            switch (InModus)
            {
                case Modi.OverView:
                    return true;
                case Modi.OverViewSdv:
                    return true;
                default:
                    return false;
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var currentTime = DateTime.Now;

            if (CultureInfo.CurrentUICulture.Name.Contains("en-"))
                Time = string.Format("{0:h:mm}", currentTime);
            else
                Time = string.Format("{0:HH:mm}", currentTime);
        }

        public async void GoToDate(DateTime dt)
        {
            try
            {
                double stamp;
                _loadEnoughCounter = 0;
                _newestGoToDateStamp = DateTime.Now.Ticks;
                stamp = DateTime.Now.Ticks;
                IsCurrentlyLoading = true;
                Days.Clear();


                if (!isStampNewest(stamp))
                    return;
                await loadFirstDay(dt, stamp);

                if (!isStampNewest(stamp))
                    return;
                await loadEnoughMoreDay(stamp);

                if (!isStampNewest(stamp))
                    return;
                await loadPastDays(3, stamp);


                IsCurrentlyLoading = false;
            }
            catch
            {
                // ignored
            }
        }

        private async Task loadFirstDay(DateTime dt, double stamp)
        {
            await loadDays(dt, 1, stamp);

            SingleDayViewModel.TappedDay = Days.First(x => x.Dt == dt);
            if (App.ViewModel.InModus == Modi.AgendaViewSdv || InModus == Modi.OverViewSdv)
            {
                ViewSwitcher.Mainpage.SingleDayViewer.PrepareForNewLoadingOfAppoinments();
                ViewSwitcher.Mainpage.SingleDayViewer.AddTappedDayAppointments();
            }

            ConflictManager.SolveConflicts();
        }

        private async Task loadEnoughMoreDay(double stamp)
        {
            if (_loadEnoughCounter > 5)
            {
                IsCurrentlyLoading = false;
                return;
            }
            _loadEnoughCounter++;
            await loadMoreDays(3, stamp);
            if (countLoadedAppointments() < 18)
            {
                await loadEnoughMoreDay(stamp);
            }
            else
                IsCurrentlyLoading = false;
        }

        private int countLoadedAppointments()
        {
            var counter = 0;
            foreach (var day in Days)
            {
                counter += day.PocalApptsOfDay.Count;
            }
            return counter;
        }

        public async Task LoadIncrementalBackwards(int howMany, double stamp)
        {
            IsCurrentlyLoading = true;
            await loadPastDays(howMany, stamp);
            IsCurrentlyLoading = false;
        }

        public async Task LoadIncrementalForward(int howMany, double stamp)
        {
            IsCurrentlyLoading = true;
            await loadMoreDays(howMany, stamp);
            IsCurrentlyLoading = false;
        }

        private async Task loadMoreDays(int howMany, double stamp)
        {
            var fromDate = Days[Days.Count - 1].Dt.AddDays(1);
            if (Days.Count > 0)
                await loadDays(fromDate, howMany, stamp);
        }

        #region LOAD

        private IReadOnlyList<Appointment> _appoinmentBuffer;
        private readonly List<PocalAppointment> _pocalAppointmentsBuffer = new List<PocalAppointment>();
        private ObservableCollection<PocalAppointment> _searchResults;

        private async Task getPocalAppointments(int howManyDays, DateTime startDay)
        {
            _appoinmentBuffer = await CalendarAPI.GetAppointments(startDay, howManyDays);
            await convertAppointmentBuffer();
        }

        private async Task convertAppointmentBuffer()
        {
            _pocalAppointmentsBuffer.Clear();
            await CalendarAPI.SetCalendars(false);

            foreach (var appt in _appoinmentBuffer)
            {
                var pocalAppt = await CreatePocalAppoinment(appt);
                _pocalAppointmentsBuffer.Add(pocalAppt);
            }
        }


        private async Task loadDays(DateTime startDay, int howManyDays, double stamp)
        {
            await getPocalAppointments(howManyDays, startDay);
            createAndAddDays(startDay, howManyDays, stamp);
        }

        private async Task loadPastDays(int howManyDays, double stamp)
        {
            var startDay = App.ViewModel.Days[0].Dt.AddDays(-howManyDays);

            await getPocalAppointments(howManyDays, startDay);
            createAndInsertPastDays(startDay, howManyDays, stamp);
        }

        #endregion

        #region Days

        private void createAndAddDays(DateTime startDay, int howManyDays, double stamp)
        {
            var dt = startDay;

            //Days.Clear();
            for (var i = 0; i < howManyDays; i++)
            {
                if (!isStampNewest(stamp))
                    return;

                // Create New Day with its Appointments
                var lastOrDefault = Days.LastOrDefault();
                if (lastOrDefault != null && (Days.Count != 0 && lastOrDefault.Dt != dt.AddDays(-1)))
                    return;
                Days.Add(new Day
                {
                    Dt = dt,
                    PocalApptsOfDay = getPocalApptsOfDay(dt)
                });

                // Sunday Attribute
                if (dt.DayOfWeek == DayOfWeek.Sunday)
                {
                    Days[i].Sunday = true;
                }

                // Iteration ++ ein Tag
                dt = dt.AddDays(1);
            }
        }

        private bool isStampNewest(double stamp)
        {
            return _newestGoToDateStamp <= stamp;
        }

        private void createAndInsertPastDays(DateTime startDay, int howManyDays, double stamp)
        {
            var dt = startDay;

            //Days.Clear();
            for (var i = 0; i < howManyDays; i++)
            {
                if (!isStampNewest(stamp))
                    return;
                // Create New Day with its Appointments
                Days.Insert(i, new Day
                {
                    Dt = dt,
                    PocalApptsOfDay = getPocalApptsOfDay(dt)
                });

                // Sunday Attribute
                if (dt.DayOfWeek == DayOfWeek.Sunday)
                {
                    Days[i].Sunday = true;
                }

                // Iteration ++ ein Tag
                dt = dt.AddDays(1);
            }
        }

        #endregion

        #region PocalAppointments

        public async Task<PocalAppointment> CreatePocalAppoinment(Appointment appt)
        {
            var cal = CalendarAPI.Calendars.FirstOrDefault(c => c.LocalId == appt.CalendarId);
            if (cal == null)
            {
                await CalendarAPI.SetCalendars(true);
                cal = CalendarAPI.Calendars.First(c => c.LocalId == appt.CalendarId);
            }

            var pocalAppt = new PocalAppointment {Appt = appt, CalColor = getAppointmentColorBrush(appt, cal)};
            return pocalAppt;
        }

        private SolidColorBrush getAppointmentColorBrush(Appointment appt, AppointmentCalendar cal)
        {
            Color calColor;
            if (appt.StartTime.Date < DateTime.Now.Date)
            {
                calColor = new Color {A = 180, B = cal.DisplayColor.B, R = cal.DisplayColor.R, G = cal.DisplayColor.G};
            }
            else
                calColor = new Color
                {
                    A = cal.DisplayColor.A,
                    B = cal.DisplayColor.B,
                    R = cal.DisplayColor.R,
                    G = cal.DisplayColor.G
                };
            var brush = new SolidColorBrush(calColor);


            return brush;
        }


        private ObservableCollection<PocalAppointment> getPocalApptsOfDay(DateTime dt)
        {
            var thisDayAppts = new ObservableCollection<PocalAppointment>();
            foreach (var pocalAppt in _pocalAppointmentsBuffer)
            {
                if (pocalAppt.IsInTimeFrameOfDate(dt.Date))
                {
                    thisDayAppts.Add(pocalAppt);
                }
            }
            thisDayAppts = sortAppointments(thisDayAppts);

            return thisDayAppts;
        }


        private ObservableCollection<PocalAppointment> sortAppointments(
            ObservableCollection<PocalAppointment> thisDayAppts)
        {
            var sorted = new ObservableCollection<PocalAppointment>();
            IEnumerable<PocalAppointment> query = thisDayAppts.OrderBy(appt => appt.StartTime);

            foreach (var appt in query)
            {
                sorted.Add(appt);
            }

            return sorted;
        }

        #endregion

        
    }
}