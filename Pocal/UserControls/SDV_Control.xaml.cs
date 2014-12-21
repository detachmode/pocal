using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using Pocal.ViewModel;

namespace Pocal
{
    public partial class SDV_Control : UserControl
    {
        internal int FirstHour = 0;
        private int LastHour = 24;

        public SDV_Control()
        {
            InitializeComponent();
            GridSetup();
        }

        public void UpdateScrollviewer()
        {
            this.SingleDayScrollViewer.UpdateLayout();
        }



        private void SingleDayScrollViewer_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (e.FinalVelocities.LinearVelocity.X > 0)
            {
                changeTappedDay(-1);
            }
            if (e.FinalVelocities.LinearVelocity.X < 0)
            {
                changeTappedDay(+1);
            }
        }

        private static void changeTappedDay(int add)
        {
            DateTime nextDate = App.ViewModel.SingleDayViewModel.TappedDay.DT.AddDays(add);
            Day newTappedDay = App.ViewModel.Days.FirstOrDefault(x => x.DT.Date == nextDate.Date);
            if (newTappedDay != null)
                App.ViewModel.SingleDayViewModel.TappedDay = newTappedDay;
            App.ViewModel.ConflictManager.solveConflicts();
        }


        public void GridSetup()
        {
            for (int i = FirstHour; i < LastHour; i++)
            {
                string str = i.ToString("00") + ":00";
                SDV_HourLine_Control control = new SDV_HourLine_Control();
                control.DataContext = new HourLine { Text = str };
                StackpanelHourLines.Children.Add(control);

            }
        }


        public void PrepareForNewLoadingOfAppoinments()
        {
            GridAppointments.Children.Clear();
        }


        public void InsertAppointments()
        {
            if (App.ViewModel.SingleDayViewModel.TappedDay == null)
                return;

            foreach (PocalAppointment pa in App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay)
            {
                if (!pa.AllDay)
                {
                    SDV_Appointment_Control control = new SDV_Appointment_Control();
                    control.DataContext = pa;
                    GridAppointments.Children.Add(control);
                }
            }

        }

        public void Update_PocalAppointment(PocalAppointment oldPA, PocalAppointment newPA)
        {
            for (int i = GridAppointments.Children.Count-1; i >= 0 ; i--)
            {
                FrameworkElement item = GridAppointments.Children[i] as FrameworkElement;
                if (item.DataContext == oldPA)
                    GridAppointments.Children.Remove(item); 
            }

            if (newPA != null && newPA.AllDay == false)
            {
                SDV_Appointment_Control control = new SDV_Appointment_Control();
                control.DataContext = newPA;
                GridAppointments.Children.Add(control);
            }

        }

    }
}
