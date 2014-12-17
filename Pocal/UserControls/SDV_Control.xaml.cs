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
        public SDV_Control()
        {
            InitializeComponent();
        }

        public void UpdateScrollviewer()
        {
            this.SingleDayScrollViewer.UpdateLayout();
        }

        public void SDV_AppointmentTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //UpdateLayout(); // Vielleicht verlangsamt das die UX! Vielleicht behebt das jedoch den TapOffset Bug.
            PocalAppointment pocalAppointment = ((FrameworkElement)sender).DataContext as PocalAppointment;


            Storyboard storyboard = ((FrameworkElement)sender).Resources["tapFeedback"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }

            Dispatcher.BeginInvoke(() =>
            {

                CalendarAPI.editAppointment(pocalAppointment);

            });


        }

        private void SDV_Hourline_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HourLine hourLine = ((FrameworkElement)sender).DataContext as HourLine;
            Dispatcher.BeginInvoke(() =>
            {
                Storyboard storyboard = ((FrameworkElement)sender).Resources["tapFeedback"] as Storyboard;
                if (storyboard != null)
                {
                    storyboard.Begin();
                }
            });
            //Dispatcher.BeginInvoke(() =>
            //{
            //Thread.Sleep(200);
            var starttime = App.ViewModel.SingleDayViewModel.getStarTimeFromHourline(hourLine.Text);
            if (starttime != null)
            {
                DateTime dt = (DateTime)starttime;
                CalendarAPI.addAppointment(dt);
            }
            //});

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

    }
}
