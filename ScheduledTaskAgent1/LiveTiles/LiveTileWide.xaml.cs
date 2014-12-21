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

namespace ScheduledTaskAgent1
{
    public partial class LiveTileWide : UserControl
    {
        public LiveTileWide()
        {
            InitializeComponent();
        }

        public void UpdateTextBox(Appointment appt)
        {

            dayOfWeekTb.Text = DateTime.Now.ToString("dddd", CultureSettings.ci).Substring(0, 2);
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
                if (appt.Location == "")
                    tbOrt.Visibility = System.Windows.Visibility.Collapsed;
                tbOrt.Text = LiveTileManager.tbOrtWide(appt);
                
                tb2.Text = LiveTileManager.tb2TextWide(appt);
                LayoutRoot.UpdateLayout();
            }

        }
    }
}
