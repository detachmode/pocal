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
using System.Windows.Media;

namespace ScheduledTaskAgent1
{
    public partial class LiveTileWide : UserControl
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
                tbOrt.Visibility = System.Windows.Visibility.Collapsed;

            tbOrt.Text = LiveTileManager.getLocationStringWide(appts[0]);

            // setzte Uhrzeit 
            tbTime.Text = LiveTileManager.getTimeStringWide(appts[0]);


            if (LiveTileManager.IsSingleLiveTileEnabled())
            {
                LayoutRoot.UpdateLayout();
                return;
            }



            // Option: mehrere Tage auf Livetile anzeigen 

            for (int i = 1; i < Math.Min(appts.Count, 3); i++)
            {
                byte alpha = 200;
                SolidColorBrush fontcolor = new SolidColorBrush(Color.FromArgb(alpha, 255, 255, 255));

                TextBlock _tbSubject = new TextBlock();
                _tbSubject.Text = appts[i].Subject;
                _tbSubject.Foreground = fontcolor;
                _tbSubject.FontSize = 30;

                TextBlock _tbTime = new TextBlock();
                _tbTime.Text = LiveTileManager.getTimeStringWide(appts[i]); ;
                _tbTime.FontSize = 30;
                _tbTime.Foreground = fontcolor;
                _tbTime.Margin = new Thickness(0, -6, 0, 24);

                otherAppointments.Items.Add(_tbSubject);
                otherAppointments.Items.Add(_tbTime);

            }
            LayoutRoot.UpdateLayout();


        }
    }
}
