﻿using System;
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
            if (DateTimeFormatInfo.CurrentInfo != null)
                dayOfWeekTb.Text = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DateTime.Now.DayOfWeek);

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
