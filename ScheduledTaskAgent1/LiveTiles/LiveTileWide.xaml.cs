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
using System.Globalization;

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
            if (CultureInfo.CurrentUICulture.Name.Contains("de-"))
                dayOfWeekTb.Text = DateTime.Now.ToString("dddd", CultureInfo.CurrentUICulture).Substring(0, 2);
            else
                dayOfWeekTb.Text = DateTime.Now.ToString("dddd", CultureInfo.CurrentUICulture).Substring(0, 3);

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
