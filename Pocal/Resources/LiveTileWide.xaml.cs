using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.ApplicationModel.Appointments;
using Pocal.Helper;

namespace Pocal.Resources
{
    public partial class LiveTileWide : UserControl
    {
        public LiveTileWide()
        {
            InitializeComponent();
        }

        public void UpdateTextBox(Appointment appt)
        {

            dayOfWeekTb.Text = DateTime.Now.DayOfWeek.ToString().Substring(0, 2);
            dayTb.Text = DateTime.Now.Day.ToString();
            if (appt == null)
            {
                tb1.Text = "";
                tb2.Text = "";
                LayoutRoot.UpdateLayout();
            }
            else
            {
                tb1.Text = appt.Subject;

                tb2.Text = LiveTileManager.tb2TextWide(appt);
                LayoutRoot.UpdateLayout();
            }

        }
    }
}
