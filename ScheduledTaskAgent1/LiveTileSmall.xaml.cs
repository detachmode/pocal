using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.ApplicationModel.Appointments;

namespace ScheduledTaskAgent1
{


    public partial class LiveTileSmall
    {
        public LiveTileSmall()
        {
            InitializeComponent();

        }

        public void UpdateDate()
        {
            // Wochentag Kürzel
            if (DateTimeFormatInfo.CurrentInfo != null)
                dayOfWeekTb.Text = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DateTime.Now.DayOfWeek);

            // Tageszahl
            dayTb.Text = DateTime.Now.Day.ToString();
           
            LayoutRoot.UpdateLayout();
           
        }

    }
}
