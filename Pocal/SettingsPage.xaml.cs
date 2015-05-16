using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Windows.ApplicationModel.Appointments;
using Pocal.Resources;
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

            if (App.ViewModel.SettingsViewModel.RestartNeeded)
            {
                App.ViewModel.SettingsViewModel.RestartNeeded = false;
                var msg = AppResources.ResourceManager.GetString("PleaseRestart",
                    AppResources.Culture);
                MessageBox.Show(msg);
                Application.Current.Terminate();
            }
        }

        private void CalendarVisibilitys()
        {
            foreach (var calendar in CalendarAPI.Calendars)
            {
                var isHidden = IsCalendarHidden(calendar);

                StackPanel stack = new StackPanel
                {
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var chk = new CheckBox
                {
                    DataContext = calendar,
                    IsChecked = !isHidden
                };

                var tb = new TextBlock
                {
                    Text = calendar.DisplayName,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 25,
                    Foreground =
                        new SolidColorBrush(new Color
                        {
                            A = 255,
                            R = calendar.DisplayColor.R,
                            G = calendar.DisplayColor.G,
                            B = calendar.DisplayColor.B
                        })
                };

                chk.Checked += SetHiddenCalendarsSettings;
                chk.Unchecked += SetHiddenCalendarsSettings;

                stack.Children.Add(chk);
                stack.Children.Add(tb);

                ListCalendarVisibility.Children.Add(stack);
            }
        }

        private void SetHiddenCalendarsSettings(object sender, RoutedEventArgs e)
        {
            var hiddenCalendars = new List<string>();
            foreach (StackPanel stack in ListCalendarVisibility.Children.Where(x => x is StackPanel))
            {
                foreach (CheckBox item in stack.Children.Where(x => x is CheckBox))
                {
                    if (item.IsChecked != false) continue;
                    var cal = (AppointmentCalendar) item.DataContext;
                    hiddenCalendars.Add(cal.LocalId);
                }
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