using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading;
using System.Windows.Media.Animation;
using System.Windows.Media;
using Pocal.Helper;
using Pocal.ViewModel;

namespace Pocal
{
    public partial class DayCard_Control : UserControl
    {
        public DayCard_Control()
        {
            InitializeComponent();
        }


        private void DayCard_ApptTap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            Storyboard storyboard = ((FrameworkElement)sender).Resources["tapFeedback"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }
            Thread.Sleep(1);
            ViewSwitcher.SetScrollToPa(((FrameworkElement)sender).DataContext as PocalAppointment);
            ViewSwitcher.From = ViewSwitcher.Sender.ApptTap;

        }



        private void DayCard_HeaderTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Storyboard storyboard = ((FrameworkElement)sender).Resources["tapFeedback"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }
            ViewSwitcher.From = ViewSwitcher.Sender.HeaderTap;
        }


        private void OpenSdvAndSetTappedDay(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                //ViewSwitcher.mainpage.addBitmapCacheToSDV();
                Thread.Sleep(150);
                ViewSwitcher.SwitchToSdv(sender);
                
                //ViewSwitcher.mainpage.removeBitmapCacheAfterAnimation();
            });


        }
    }
}
