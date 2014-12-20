using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Appointments;
using System.Windows.Media;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using Pocal.Helper;
using System.Windows;
using System.Threading;
using System.Windows.Threading;


namespace Pocal.ViewModel
{

    public class MainViewModel : ViewModelBase
    {

        public enum Modi { AgendaView, OverView, OverViewSDV, AgendaViewSDV };
        public Modi InModus = Modi.AgendaView;

        public bool IsCurrentlyLoading = false;


        public ObservableCollection<Day> _days;
        public ObservableCollection<Day> Days
        {
            get
            {
                return _days;
            }
            set
            {
                if (value != _days)
                {
                    _days = value;
                    NotifyPropertyChanged("Days");
                }
            }
        }



        public SingleDayViewVM SingleDayViewModel { get; private set; }
        public ConflictManager ConflictManager { get; private set; }

        private Day _dayAtPointer;
        public Day DayAtPointer
        {
            get
            {
                return _dayAtPointer;
            }
            set
            {
                if (value != _dayAtPointer)
                {
                    _dayAtPointer = value;
                    NotifyPropertyChanged("DayAtPointer");
                }
            }
        }


        private string _time;
        public string Time
        {
            get
            {
                return _time;
            }
            private set
            {
                if (value != _time)
                {
                    _time = value;
                    NotifyPropertyChanged("Time");
                }
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            Time = currentTime.Hour + ":" + currentTime.Minute.ToString("00");



        }

        public MainViewModel()
        {

            this.Days = new ObservableCollection<Day>();
            SingleDayViewModel = new SingleDayViewVM();
            ConflictManager = new ConflictManager();

            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000); // TODO performance
            dispatcherTimer.Start();

            #region DESIGN TIME DATA
            if (DesignerProperties.IsInDesignTool)
            {
                //	//CREATE DESIGN TIME DATA HERE
                DateTime start = new DateTime(2014, 11, 07);
                DateTime dt = start - start.TimeOfDay;

                DateTime dt0 = dt;
                DateTime dt1 = dt0.AddDays(1);
                DateTime dt2 = dt0.AddDays(2);
                DateTime dt3 = dt0.AddDays(3);
                DateTime dt4 = dt0.AddDays(4);
                TimeSpan ts = new TimeSpan(1, 30, 0);
                TimeSpan ts2 = new TimeSpan(2, 0, 0);
                TimeSpan ts3 = new TimeSpan(3, 0, 0);
                CultureInfo ci = new CultureInfo("de-DE");

                //var appointment = new Windows.ApplicationModel.Appointments.Appointment();
                ObservableCollection<Appointment> DesignDataDayappts = new ObservableCollection<Appointment>();

                SolidColorBrush CalColorGreen = new SolidColorBrush(Color.FromArgb(255, 100, 255, 0));
                SolidColorBrush CalColorYellow = new SolidColorBrush(Colors.Yellow);
                SolidColorBrush CalColorOrange = new SolidColorBrush(Colors.Orange);
                SolidColorBrush CalColorRed = new SolidColorBrush(Colors.Red);




                DesignDataDayappts.Add(new Appointment { Subject = "Gameplay Programming", StartTime = dt2.AddHours(8.5), Duration = ts3 });
                DesignDataDayappts.Add(new Appointment { Subject = "IT Security", StartTime = dt2.AddHours(11.75), Duration = ts });


                DesignDataDayappts.Add(new Appointment { Subject = "Structered Data and Application", StartTime = dt3.AddHours(0), AllDay = false, Location = "HdM Raum 011", Details = "EEine Notiz wie keine Zweite.Und auch keine Dritte ! Oder eine Vierte. Oder eine Fünfte Eine Notiz wie keine Zweite.Und auch keine Dritte ! Oder eine Vierte. Oder eine Fünfte ine Notiz wie keine Zweite.Und auch keine Dritte ! Oder eine Vierte. Oder eine Fünfte.Eine Notiz wie keine Zweite.Und auch keine Dritte ! Oder eine Vierte. Oder eine Fünfte", Duration = ts2 });
                //DesignDataDayappts.Add(new Appointment { Subject = "Artificial Intelligence for Games", StartTime = dt3.AddHours(14), Duration = ts3 });
                DesignDataDayappts.Add(new Appointment { Subject = "Exercises Structered Data and Application", StartTime = dt4.AddHours(1.5), AllDay = false, Duration = ts3 });
                DesignDataDayappts.Add(new Appointment { Subject = "IT Security", StartTime = dt4.AddHours(2.75), Duration = ts });
                DesignDataDayappts.Add(new Appointment { Subject = "BWL für Informatiker", StartTime = dt4.AddHours(16.00), Duration = ts });



                DesignDataDayappts.Add(new Appointment { Subject = "Geburtstag von Peter Pan", StartTime = dt.AddHours(0), AllDay = true });
                DesignDataDayappts.Add(new Appointment { Subject = "Geburtstag von Bob Marley", StartTime = dt.AddHours(0), AllDay = true });
                DesignDataDayappts.Add(new Appointment { Subject = "Essen", StartTime = dt.AddHours(9), Duration = ts, Location = "Stuttgart" });
                DesignDataDayappts.Add(new Appointment { Subject = "Einkaufen", StartTime = dt.AddHours(11.3), Duration = ts });
                DesignDataDayappts.Add(new Appointment { Subject = "Mom Anrufen", StartTime = dt.AddHours(14.5), Duration = ts });

                ObservableCollection<PocalAppointment> DesignDataDayPocalappts = new ObservableCollection<PocalAppointment>();

                ObservableCollection<PocalAppointment> DesignDataDay2_Pocalappts = new ObservableCollection<PocalAppointment>();
                DesignDataDay2_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[6], CalColor = CalColorOrange });
                DesignDataDay2_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[7], CalColor = CalColorOrange });
                DesignDataDay2_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[8], CalColor = CalColorOrange });

                DesignDataDay2_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[0], CalColor = CalColorRed });

                DesignDataDay2_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[9], CalColor = CalColorOrange });
                DesignDataDay2_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[10], CalColor = CalColorOrange });

                DesignDataDay2_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[1], CalColor = CalColorRed });


                ObservableCollection<PocalAppointment> DesignDataDay3_Pocalappts = new ObservableCollection<PocalAppointment>();
                DesignDataDay3_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[6], CalColor = CalColorRed, AllDay = true });
                DesignDataDay3_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[2], CalColor = CalColorRed, AllDay = false });
                DesignDataDay3_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[3], CalColor = CalColorRed, AllDay = false });


                ObservableCollection<PocalAppointment> DesignDataDay4_Pocalappts = new ObservableCollection<PocalAppointment>();
                DesignDataDay4_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[4], CalColor = CalColorRed });
                DesignDataDay4_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[5], CalColor = CalColorRed });
                DesignDataDay4_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[6], CalColor = CalColorRed });
                //DesignDataDayPocalappts.Add(new PocalAppointment(DesignDataDayappts[2], Color.FromArgb(155, 55, 55, 55)));

                Days.Add(new Day { DT = dt2, PocalApptsOfDay = DesignDataDay2_Pocalappts, Sunday = false });
                Days.Add(new Day { DT = dt3, PocalApptsOfDay = DesignDataDay3_Pocalappts, Sunday = false });
                Days.Add(new Day { DT = dt4, PocalApptsOfDay = DesignDataDay4_Pocalappts, Sunday = false });



                //DesignDataDayPocalappts1.Add(new PocalAppointment { Appt = DesignDataDayappts[4], CalColor = CalColorYellow });
                //dt = DateTime.Now.AddDays(1);
                //Days.Add(new Day { DT = dt, PocalApptsOfDay = DesignDataDayPocalappts1 });

                //ObservableCollection<PocalAppointment> DesignDataDayPocalappts2 = new ObservableCollection<PocalAppointment>();
                //dt = dt.AddDays(1);
                //Days.Add(new Day { DT = dt, PocalApptsOfDay = DesignDataDayPocalappts2 });
                //dt = dt.AddDays(1);
                //Days.Add(new Day { DT = dt, PocalApptsOfDay = DesignDataDayPocalappts });




                SingleDayViewModel.TappedDay = Days[0];
                DayAtPointer = new Day { DT = dt.AddHours(24) };

            }
            #endregion

        }

        private double newestGoToDateStamp = DateTime.Now.Ticks;
        private Object mylock = new Object();

        public async void GoToDate(DateTime dt)
        {
            try
            {
                double stamp;
                newestGoToDateStamp = DateTime.Now.Ticks;
                stamp = DateTime.Now.Ticks;
                IsCurrentlyLoading = true;
                this.Days.Clear();




                if (!isStampNewest(stamp))
                    return;
                else
                    await loadFirstDay(dt, stamp);

                if (!isStampNewest(stamp))
                    return;
                else
                    await loadEnoughMoreDay(stamp);

                if (!isStampNewest(stamp))
                    return;
                else
                    await loadPastDays(3, stamp);


                IsCurrentlyLoading = false;
            }
            catch (Exception)
            {
            }

        }

        private async Task loadFirstDay(DateTime dt, double stamp)
        {
            await loadDays(dt, 1, stamp);
            SingleDayViewModel.TappedDay = Days.First(x => x.DT == dt);
            ConflictManager.solveConflicts();
        }

        private async Task loadEnoughMoreDay(double stamp)
        {
            await LoadMoreDays(3, stamp);
            if (countLoadedAppointments() < 18)
            {
                await LoadMoreDays(3, stamp);
            }
        }

        private int countLoadedAppointments()
        {
            int counter = 0;
            foreach (Day day in Days)
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
            await LoadMoreDays(howMany, stamp);
            IsCurrentlyLoading = false;
        }


        private async Task LoadMoreDays(int howMany, double stamp)
        {
            DateTime fromDate = Days[Days.Count - 1].DT.AddDays(1);
            if (Days.Count > 0)
                await loadDays(fromDate, howMany, stamp);
        }


        #region LOAD
        private IReadOnlyList<Appointment> appoinmentBuffer;
        private List<PocalAppointment> pocalAppointmentsBuffer = new List<PocalAppointment>();

        private async Task getPocalAppointments(int howManyDays, DateTime startDay)
        {
            appoinmentBuffer = await CalendarAPI.getAppointments(startDay, howManyDays);
            await convertAppointmentBuffer();
        }

        private async Task convertAppointmentBuffer()
        {
            pocalAppointmentsBuffer.Clear();
            await CalendarAPI.setCalendars();

            foreach (var appt in appoinmentBuffer)
            {
                PocalAppointment pocalAppt = CreatePocalAppoinment(appt);
                pocalAppointmentsBuffer.Add(pocalAppt);
            }
        }




        private async Task loadDays(DateTime startDay, int howManyDays, double stamp)
        {
            await getPocalAppointments(howManyDays, startDay);
            createAndAddDays(startDay, howManyDays, stamp);



        }

        private async Task loadPastDays(int howManyDays, double stamp)
        {

            DateTime startDay = App.ViewModel.Days[0].DT.AddDays(-howManyDays);

            await getPocalAppointments(howManyDays, startDay);
            createAndInsertPastDays(startDay, howManyDays, stamp);


        }


        #endregion

        #region Days


        private void createAndAddDays(DateTime startDay, int howManyDays, double stamp)
        {
            DateTime dt = startDay;
            CultureInfo ci = new CultureInfo("de-DE");

            //Days.Clear();
            for (int i = 0; i < howManyDays; i++)
            {
                if (!isStampNewest(stamp))
                    return;

                // Create New Day with its Appointments
                if (Days.Count != 0 && Days.LastOrDefault().DT != dt.AddDays(-1))
                    return;
                Days.Add(new Day()
                {
                    DT = dt,
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
            return newestGoToDateStamp <= stamp;
        }

        private void createAndInsertPastDays(DateTime startDay, int howManyDays, double stamp)
        {
            DateTime dt = startDay;
            CultureInfo ci = new CultureInfo("de-DE");

            //Days.Clear();
            for (int i = 0; i < howManyDays; i++)
            {
                if (!isStampNewest(stamp))
                    return;
                // Create New Day with its Appointments
                Days.Insert(i, new Day()
                {
                    DT = dt,
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
        private Dictionary<AppointmentCalendar, SolidColorBrush> calColors = new Dictionary<AppointmentCalendar, SolidColorBrush>();

        public PocalAppointment CreatePocalAppoinment(Appointment appt)
        {
            var cal = CalendarAPI.calendars.First(c => c.LocalId == appt.CalendarId);

            PocalAppointment pocalAppt = new PocalAppointment { Appt = appt, CalColor = getAppointmentColorBrush(appt, cal) };
            return pocalAppt;
        }

        private SolidColorBrush getAppointmentColorBrush(Appointment appt, AppointmentCalendar cal)
        {
            SolidColorBrush brush = null;
            //calColors.TryGetValue(cal, out brush);

            //if (brush == null)
            //{
            Color calColor;
            if (appt.StartTime.Date < DateTime.Now.Date)
            {
                calColor = new System.Windows.Media.Color() { A = 180, B = cal.DisplayColor.B, R = cal.DisplayColor.R, G = cal.DisplayColor.G };
            }
            else
                calColor = new System.Windows.Media.Color() { A = cal.DisplayColor.A, B = cal.DisplayColor.B, R = cal.DisplayColor.R, G = cal.DisplayColor.G };
            brush = new SolidColorBrush(calColor);
            //calColors.Add(cal, brush);
            //return brush;
            //}

            return brush;
        }



        private ObservableCollection<PocalAppointment> getPocalApptsOfDay(DateTime dt)
        {

            ObservableCollection<PocalAppointment> thisDayAppts = new ObservableCollection<PocalAppointment>();
            foreach (PocalAppointment pocalAppt in pocalAppointmentsBuffer)
            {
                if (pocalAppt.isInTimeFrameOfDate(dt.Date))
                {
                    thisDayAppts.Add(pocalAppt);
                }
            }
            thisDayAppts = sortAppointments(thisDayAppts);

            return thisDayAppts;
        }


        private ObservableCollection<PocalAppointment> sortAppointments(ObservableCollection<PocalAppointment> thisDayAppts)
        {
            ObservableCollection<PocalAppointment> sorted = new ObservableCollection<PocalAppointment>();
            IEnumerable<PocalAppointment> query = thisDayAppts.OrderBy(appt => appt.StartTime);

            foreach (PocalAppointment appt in query)
            {
                sorted.Add(appt);
            }

            return sorted;
        }

        #endregion



    }


}
