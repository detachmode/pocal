using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Pocal.ViewModel;
using System.Windows.Media.Animation;

namespace Pocal
{
    public partial class SDV_AllDay_Control : UserControl
    {
        public SDV_AllDay_Control()
        {
            InitializeComponent();
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
    }
}
