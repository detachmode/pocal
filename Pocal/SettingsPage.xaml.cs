using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Windows.ApplicationModel.Appointments;
using ScheduledTaskAgent1;
using Shared.Helper;

namespace Pocal
{
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel.SettingsViewModel;

            CalendarVisibilitys();

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            LiveTileManager.UpdateTileFromForeground();
        }

        private void CalendarVisibilitys()
        {
            foreach (var calendar in CalendarAPI.Calendars)
            {
                var isHidden = IsCalendarHidden(calendar);
                var chk = new CheckBox
                    {
                        DataContext = calendar,
                        Content =  calendar.DisplayName,           
                        Foreground = new SolidColorBrush(new Color(){ A= 255, R = calendar.DisplayColor.R , G = calendar.DisplayColor.G, B = calendar.DisplayColor.B }),
                        IsChecked = !isHidden                       
                    };
                chk.Checked += SetHiddenCalendarsSettings;
                chk.Unchecked += SetHiddenCalendarsSettings;
                
                ListCalendarVisibility.Children.Add(chk);
            }
            

        }

        void SetHiddenCalendarsSettings(object sender, RoutedEventArgs e)
        {
            var hiddenCalendars = new List<string>();
            foreach (CheckBox item in ListCalendarVisibility.Children)
            {
                if (item.IsChecked != false) continue;
                var cal = (AppointmentCalendar)item.DataContext;
                hiddenCalendars.Add(cal.LocalId);
            }
            App.ViewModel.SettingsViewModel.HiddenCalendars = hiddenCalendars;
        }


        private static bool IsCalendarHidden(AppointmentCalendar calendar)
        {
            if (calendar == null) throw new ArgumentNullException("calendar");
            return App.ViewModel.SettingsViewModel.HiddenCalendars.Any(id => id == calendar.LocalId);
        }

        private void mySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mySlider == null) return;
            mySlider.Value = Math.Round(e.NewValue);
        }
    }
}