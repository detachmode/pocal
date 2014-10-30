﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Appointments;
using System.Windows.Media;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
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

namespace Pocal.ViewModel
{

    public class MainVM : ViewModelBase
    {

        public ObservableCollection<PocalAppointment> AllPocalAppointments = new ObservableCollection<PocalAppointment>();
        public SingleDayViewVM SingleDayViewModel { get; private set; }

        private IReadOnlyList<Appointment> appoinmentBuffer;

        #region RELOAD
        public async void ReloadPocalApptsAndDays()
        {
            DateTime start = DateTime.Now;

            appoinmentBuffer = await CalendarAPI.getAppointments();

            TimeSpan delta = DateTime.Now - start;
            Debug.WriteLine("Timestamp delta: " + (delta.Seconds + (1.0 / 1000) * delta.Milliseconds) + " sekunden");

           
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
        public PocalAppointment createPocalAppoinment(Appointment appt)
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
                if (pocalAppt.StartTime.Date == dt.Date)
                {
                    thisDayAppts.Add(pocalAppt);
                }
            }
            thisDayAppts = sortAppointments(thisDayAppts);

            return thisDayAppts;
        }

        public ObservableCollection<PocalAppointment> sortAppointments(ObservableCollection<PocalAppointment> thisDayAppts)
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
            if (DesignerProperties.IsInDesignTool)
            {
                //	//CREATE DESIGN TIME DATA HERE
                DateTime dt = DateTime.Now - DateTime.Now.TimeOfDay;
                DateTime dt0 = new DateTime(2014, 10, 20);
                DateTime dt1 = dt0.AddDays(1);
                DateTime dt2 = dt0.AddDays(2);
                DateTime dt3 = dt0.AddDays(3);
                DateTime dt4 = dt0.AddDays(4);
                TimeSpan ts = new TimeSpan(1, 30, 0);
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

               
                DesignDataDayappts.Add(new Appointment { Subject = "Structered Data and Application", StartTime = dt3.AddHours(1.5), Duration = ts });
                DesignDataDayappts.Add(new Appointment { Subject = "Artificial Intelligence for Games", StartTime = dt3.AddHours(14.25), Duration = ts3 });

               
                DesignDataDayappts.Add(new Appointment { Subject = "Exercises Structered Data and Application", StartTime = dt4.AddHours(8.5), Duration = ts });
                DesignDataDayappts.Add(new Appointment { Subject = "IT Security", StartTime = dt4.AddHours(11.75), Duration = ts });
                DesignDataDayappts.Add(new Appointment { Subject = "BWL für Informatiker", StartTime = dt4.AddHours(16.00), Duration = ts });



                DesignDataDayappts.Add(new Appointment { Subject = "Geburtstag von Peter Pan", StartTime = dt.AddHours(0), AllDay = true });
                DesignDataDayappts.Add(new Appointment { Subject = "Geburtstag von Bob Marley", StartTime = dt.AddHours(0), AllDay = true });
                DesignDataDayappts.Add(new Appointment { Subject = "Essen", StartTime = dt.AddHours(9), Duration = ts, Location = "Stuttgart" });
                DesignDataDayappts.Add(new Appointment { Subject = "Einkaufen", StartTime = dt.AddHours(11.3), Duration = ts });
                DesignDataDayappts.Add(new Appointment { Subject = "Mom Anrufen", StartTime = dt.AddHours(14.5), Duration = ts });

                ObservableCollection<PocalAppointment> DesignDataDayPocalappts = new ObservableCollection<PocalAppointment>();

                ObservableCollection<PocalAppointment> DesignDataDay2_Pocalappts = new ObservableCollection<PocalAppointment>();
                DesignDataDay2_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[7], CalColor = CalColorOrange });
                DesignDataDay2_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[8], CalColor = CalColorOrange });

                DesignDataDay2_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[0], CalColor = CalColorRed });

                DesignDataDay2_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[9], CalColor = CalColorOrange });
                DesignDataDay2_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[10], CalColor = CalColorOrange });

                DesignDataDay2_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[1], CalColor = CalColorRed });


                ObservableCollection<PocalAppointment> DesignDataDay3_Pocalappts = new ObservableCollection<PocalAppointment>();
                DesignDataDay3_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[2], CalColor = CalColorRed });
                DesignDataDay3_Pocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[3], CalColor = CalColorRed });

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

                SingleDayViewModel.TappedDay = Days[1];


                CurrentTop = new Day { DT = dt.AddHours(24) };

            }
            #endregion

        }

    }
}
