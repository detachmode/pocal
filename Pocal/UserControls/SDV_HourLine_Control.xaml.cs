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
using Pocal.Helper;

namespace Pocal
{
    public partial class SDV_HourLine_Control : UserControl
    {
        public SDV_HourLine_Control(string hourLineText)
        {
            InitializeComponent();
            textbox.Text = hourLineText;
        }



        private void SDV_Hourline_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HourLine hourLine = ((FrameworkElement)sender).DataContext as HourLine;
            
            // Starte Animation
            Dispatcher.BeginInvoke(() =>
            {
                Storyboard storyboard = ((FrameworkElement)sender).Resources["tapFeedback"] as Storyboard;
                if (storyboard != null)
                {
                    storyboard.Begin();
                }
            });

            // Finde die Uhrzeit der angetippten Stelle heraus und übergebe diese Uhrzeit der API
            var starttime = App.ViewModel.SingleDayViewModel.GetStarTimeFromHourline(hourLine);
            if (starttime != null)
            {
                DateTime dt = (DateTime)starttime;
                PocalAppointmentHelper.addAppointment(dt);
            }
 

        }
    }

}
