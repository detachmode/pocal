using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Windows.Controls;
using Windows.ApplicationModel.Appointments;


namespace ScheduledTaskAgent1
{

    
    public partial class LiveTile : UserControl
    {
        public LiveTile()
        {
            InitializeComponent();

        }

        public void UpdateTextBox(List<Appointment> appts)
        {

            if (CultureInfo.CurrentUICulture.Name.Contains("de-"))
                dayOfWeekTb.Text = DateTime.Now.ToString("dddd", CultureInfo.CurrentUICulture).Substring(0, 2);
            else
                dayOfWeekTb.Text = DateTime.Now.ToString("dddd", CultureInfo.CurrentUICulture).Substring(0, 3);

            dayTb.Text = DateTime.Now.Day.ToString();


            if (appts.Count == 0)
            {
                tb1.Text = "";
                tb2.Text = "";
                LayoutRoot.UpdateLayout();
            }
            else 
            {
                tb1.Text = appts[0].Subject;   
                tb2.Text = LiveTileManager.tb2TextNormal(appts[0]);
                LayoutRoot.UpdateLayout();
            }
           
        }

    }
}
