using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.ApplicationModel.Appointments;

namespace ScheduledTaskAgent1
{
    public partial class LiveTileWide
    {
        public LiveTileWide()
        {
            InitializeComponent();
        }

        public void UpdateTextBox(List<Appointment> appts)
        {
            // setzte Wochentag Kürzel: deutsch = 2 Buchstaben, andere 3 Buchstaben
            if (CultureInfo.CurrentUICulture.Name.Contains("de-"))
                dayOfWeekTb.Text = DateTime.Now.ToString("dddd", CultureInfo.CurrentUICulture).Substring(0, 2);
            else
                dayOfWeekTb.Text = DateTime.Now.ToString("dddd", CultureInfo.CurrentUICulture).Substring(0, 3);

            // setzte Tageszahl
            dayTb.Text = DateTime.Now.Day.ToString();



            // Wenn keine Termine in Liste, dann setzte die Felder auf leer
            if (appts.Count == 0)
            {
                tbSubject.Text = "";
                tbTime.Text = "";
                LayoutRoot.UpdateLayout();
                return;
            }


            // 1. Termin:

            // setzte Subject 
            tbSubject.Text = appts[0].Subject;

            // setzte Location
            if (appts[0].Location == "")
                tbOrt.Visibility = Visibility.Collapsed;
            else
            {
                otherAppointments.Margin = new Thickness(0, 10, 0, 10);
            }
              


            tbOrt.Text = LiveTileManager.GetLocationStringWide(appts[0]);

            // setzte Uhrzeit 
            tbTime.Text = LiveTileManager.GetTimeStringWide(appts[0]);


            if (LiveTileManager.IsSingleLiveTileEnabled())
            {
                LayoutRoot.UpdateLayout();
                return;
            }



            // Option: mehrere Tage auf Livetile anzeigen 


            const byte alpha = 200;

            for (var i = 1; i < Math.Min(appts.Count, 3); i++)
            {
                var subjectTextblock = new TextBlock();
                var timeTextBlock = new TextBlock();
                var fontcolor = new SolidColorBrush(Color.FromArgb(alpha, 255, 255, 255));

                subjectTextblock.Text = appts[i].Subject;
                subjectTextblock.Foreground = fontcolor;
                subjectTextblock.FontSize = 30;

                timeTextBlock.Text = LiveTileManager.GetTimeStringWide(appts[i]); 
                timeTextBlock.FontSize = 30;
                timeTextBlock.Foreground = fontcolor;
                timeTextBlock.Margin = new Thickness(0, -6, 0, 10);

                otherAppointments.Items.Add(subjectTextblock);
                otherAppointments.Items.Add(timeTextBlock);

            }
            LayoutRoot.UpdateLayout();


        }
    }
}
