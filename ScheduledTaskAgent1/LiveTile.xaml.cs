using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.ApplicationModel.Appointments;

namespace ScheduledTaskAgent1
{

    
    public partial class LiveTile
    {
        public LiveTile()
        {
            InitializeComponent();

        }

        public void UpdateTextBox(List<Appointment> appts)
        {
            // Wochentag Kürzel
            if (CultureInfo.CurrentUICulture.Name.Contains("de-"))
                dayOfWeekTb.Text = DateTime.Now.ToString("dddd", CultureInfo.CurrentUICulture).Substring(0, 2);
            else
                dayOfWeekTb.Text = DateTime.Now.ToString("dddd", CultureInfo.CurrentUICulture).Substring(0, 3);

            // Tageszahl
            dayTb.Text = DateTime.Now.Day.ToString();

            // anstehender Termin
            if (appts.Count == 0)
            {
                tbSubject.Text = "";
                tbTime.Text = "";
            }
            else 
            {
                tbSubject.Text = appts[0].Subject;   
                tbTime.Text = LiveTileManager.GetTimeStringNormal(appts[0]);
                
            }
            LayoutRoot.UpdateLayout();
           
        }

    }
}
