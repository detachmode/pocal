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

                if (appts[0].Location == "")
                    tbOrt.Visibility = System.Windows.Visibility.Collapsed;
                tbOrt.Text = LiveTileManager.tbOrtWide(appts[0]);

                tb2.Text = LiveTileManager.tb2TextWide(appts[0]);


                if (LiveTileManager.IsSingleLiveTileEnabled())
                {
                    LayoutRoot.UpdateLayout();
                    return;
                }
                    

                for (int i = 1; i < Math.Min(appts.Count, 3); i++)
                {
                    byte grayColor = 255;
                    SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(200,grayColor, grayColor, grayColor));
                    TextBlock tb = new TextBlock();
                    tb.Text = appts[i].Subject;
                    tb.Foreground = brush;
                    tb.FontSize = 30;

                    TextBlock tbTime = new TextBlock();
                    tbTime.Text = LiveTileManager.tb2TextWide(appts[i]); ;

                    tbTime.FontSize = 30;
                    tbTime.Foreground = brush;
                    tbTime.Margin = new Thickness(0, -6, 0, 24);

                    otherAppointments.Items.Add(tb);
                    otherAppointments.Items.Add(tbTime);

                }
                LayoutRoot.UpdateLayout();
            }

        }
    }
}
