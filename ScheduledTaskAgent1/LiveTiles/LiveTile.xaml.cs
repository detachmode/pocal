using System;
using System.Globalization;
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
               
                tb2.Text = LiveTileManager.tb2TextNormal(appt);
                LayoutRoot.UpdateLayout();
            }
           
        }

    }
}
