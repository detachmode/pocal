using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using Windows.ApplicationModel.Appointments;

namespace Pocal
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel.SettingsViewModel;

            CalendarVisibilitys();

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ScheduledTaskAgent1.LiveTileManager.UpdateTileFromForeground();
        }

        private void CalendarVisibilitys()
        {
            foreach (var calendar in Shared.Helper.CalendarAPI.Calendars)
            {
                bool isHidden = isCalendarHidden(calendar);
                CheckBox chk = new CheckBox
                    {
                        DataContext = calendar,
                        Content =  calendar.DisplayName,           
                        Foreground = new SolidColorBrush(new Color(){ A= 255, R = calendar.DisplayColor.R , G = calendar.DisplayColor.G, B = calendar.DisplayColor.B }),
                        IsChecked = !isHidden                       
                    };
                chk.Checked += setHiddenCalendarsSettings;
                chk.Unchecked += setHiddenCalendarsSettings;
                
                ListCalendarVisibility.Items.Add(chk);
            }

        }

        void setHiddenCalendarsSettings(object sender, RoutedEventArgs e)
        {
            List<string> hiddenCalendars = new List<string>();
            foreach (CheckBox item in ListCalendarVisibility.Items)
            {
                if (item.IsChecked == false)
                {
                    AppointmentCalendar cal = (AppointmentCalendar)item.DataContext;
                    hiddenCalendars.Add(cal.LocalId);
                }
            }
            App.ViewModel.SettingsViewModel.HiddenCalendars = hiddenCalendars;
        }


        private bool isCalendarHidden(Windows.ApplicationModel.Appointments.AppointmentCalendar calendar)
        {
           
            foreach (string id in App.ViewModel.SettingsViewModel.HiddenCalendars)
            {
                if (id == calendar.LocalId)
                    return true;

            }
            return false;
        }


    }
}