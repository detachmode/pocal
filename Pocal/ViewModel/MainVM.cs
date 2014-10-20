using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Appointments;
using System.Windows.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pocal.ViewModel
{

    public class MainVM : ViewModelBase
    {

        internal ObservableCollection<PocalAppointment> AllPocalAppointments = new ObservableCollection<PocalAppointment>();
        public SingleDayViewVM SingleDayViewModel { get; private set; }

        private IReadOnlyList<Appointment> appoinmentBuffer;

        #region RELOAD
        public async void reloadPocalApptsAndDays()
        {
            appoinmentBuffer = await CalendarAPI.getAppointments();

            if (appoinmentBuffer.Any())
            {
                await createAllPocalApptointments();
                createDays();

                // Aufgrund mangelnder Umsetzung von MVVM
                SingleDayViewModel.updateTappedDay();
            }
        }
        #endregion

        #region PocalAppointments

        private async Task createAllPocalApptointments()
        {
            AllPocalAppointments.Clear();
            await CalendarAPI.setCalendars();

            foreach (var appt in appoinmentBuffer)
            {
                PocalAppointment pocalAppt = createPocalAppoinment(appt);
                AllPocalAppointments.Add(pocalAppt);
            }
        }
        private PocalAppointment createPocalAppoinment(Appointment appt)
        {
            var cal = CalendarAPI.calendars.First(c => c.LocalId == appt.CalendarId);
            var calColor = new System.Windows.Media.Color() { A = cal.DisplayColor.A, B = cal.DisplayColor.B, R = cal.DisplayColor.R, G = cal.DisplayColor.G };

            PocalAppointment pocalAppt = new PocalAppointment { Appt = appt, CalColor = new SolidColorBrush(calColor) };
            return pocalAppt;
        }

        private ObservableCollection<PocalAppointment> getPocalApptsOfDay(DateTime dt)
        {

            ObservableCollection<PocalAppointment> thisDayAppts = new ObservableCollection<PocalAppointment>();
            foreach (PocalAppointment pocalAppt in AllPocalAppointments)
            {
                if (pocalAppt.StartTime.Day == dt.Day)
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

        #region Days
        internal int howManyDays = 30;
        public ObservableCollection<Day> Days { get; private set; }
       
        public void createDays()
        {
            DateTime dt = DateTime.Now;
            CultureInfo ci = new CultureInfo("de-DE");

            Days.Clear();
            for (int i = 0; i < howManyDays; i++)
            {
                // Create New Day with its Appointments
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
        #endregion


        private Day _currentTop;
        public Day CurrentTop
        {
            get
            {
                return _currentTop;
            }
            set
            {
                if (value != _currentTop)
                {
                    _currentTop = value;
                    NotifyPropertyChanged("CurrentTop");
                }
            }
        }

        public MainVM()
        {
            this.Days = new ObservableCollection<Day>();
            SingleDayViewModel = new SingleDayViewVM();

            #region DESIGN TIME DATA
            //if (DesignerProperties.IsInDesignTool)
            {
                //	//CREATE DESIGN TIME DATA HERE
                DateTime dt = DateTime.Now - DateTime.Now.TimeOfDay;
                TimeSpan ts = new TimeSpan(1, 30, 0);
                CultureInfo ci = new CultureInfo("de-DE");

                //var appointment = new Windows.ApplicationModel.Appointments.Appointment();
                ObservableCollection<Appointment> DesignDataDayappts = new ObservableCollection<Appointment>();

                SolidColorBrush CalColorGreen = new SolidColorBrush(Color.FromArgb(255, 100, 255, 0));
                SolidColorBrush CalColorYellow = new SolidColorBrush(Colors.Yellow);
                SolidColorBrush CalColorOrange = new SolidColorBrush(Colors.Orange);

                DesignDataDayappts.Add(new Appointment { Subject = "Geburtstag", StartTime = dt.AddHours(0), AllDay = true });
                DesignDataDayappts.Add(new Appointment { Subject = "Geburtstag", StartTime = dt.AddHours(0), AllDay = true });
                DesignDataDayappts.Add(new Appointment { Subject = "Essen", StartTime = dt.AddHours(9), Duration = ts, Location = "Stuttgart" });
                DesignDataDayappts.Add(new Appointment { Subject = "Einkaufen", StartTime = dt.AddHours(11.3), Duration = ts });
                DesignDataDayappts.Add(new Appointment { Subject = "Mom Anrufen", StartTime = dt.AddHours(14.5), Duration = ts });

                ObservableCollection<PocalAppointment> DesignDataDayPocalappts = new ObservableCollection<PocalAppointment>();
                DesignDataDayPocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[0], CalColor = CalColorOrange });
                DesignDataDayPocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[1], CalColor = CalColorOrange });
                DesignDataDayPocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[2], CalColor = CalColorGreen });
                //DesignDataDayPocalappts.Add(new PocalAppointment(DesignDataDayappts[2], Color.FromArgb(155, 55, 55, 55)));
                Days.Add(new Day { DT = dt, PocalApptsOfDay = DesignDataDayPocalappts, Sunday = true });

                ObservableCollection<PocalAppointment> DesignDataDayPocalappts1 = new ObservableCollection<PocalAppointment>();
                DesignDataDayPocalappts1.Add(new PocalAppointment { Appt = DesignDataDayappts[4], CalColor = CalColorYellow });
                dt = DateTime.Now.AddDays(1);
                Days.Add(new Day { DT = dt, PocalApptsOfDay = DesignDataDayPocalappts1 });

                ObservableCollection<PocalAppointment> DesignDataDayPocalappts2 = new ObservableCollection<PocalAppointment>();
                dt = dt.AddDays(1);
                Days.Add(new Day { DT = dt, PocalApptsOfDay = DesignDataDayPocalappts2 });
                dt = dt.AddDays(1);
                Days.Add(new Day { DT = dt, PocalApptsOfDay = DesignDataDayPocalappts });

                SingleDayViewModel.TappedDay = Days[1];


                CurrentTop = new Day { DT = dt.AddHours(24) };

            }
            #endregion

        }

    }
}
