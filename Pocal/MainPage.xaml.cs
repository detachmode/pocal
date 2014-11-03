using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Pocal.ViewModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Pocal
{
    public partial class MainPage : PhoneApplicationPage
    {

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;


            // Meine SETUP Funktionen
            //watchPositionOfLongListSelector();
            App.ViewModel.ReloadPocalApptsAndDays();

            AgendaViewListbox.ManipulationStateChanged += AgendaScrolling_WhileSingleDayViewIsOpen_Fix;

            App.ViewModel.SingleDayViewModel.TriggerScrollToOffset += SingleDayViewModel_IsUpdated;
            //SingleDayScrollViewer.SizeChanged += scrollToDefaultOffset;


        }

        private void AgendaScrolling_WhileSingleDayViewIsOpen_Fix(object sender, EventArgs e)
        {
            if (SingleDayView.Visibility == Visibility.Visible)
                VisualStateManager.GoToState(this, "Close", true);

        }




        #region Position im Longlistselektor bestimmen

        // Wird benötigt um die Position im Longlistselektor zu bestimmen um damit DeltaDays auszurechnen.
        private Dictionary<object, ContentPresenter> items = new Dictionary<object, ContentPresenter>();

        private void watchPositionOfLongListSelector()
        {
            AgendaViewListbox.ItemRealized += LLS_ItemRealized;
            AgendaViewListbox.ItemUnrealized += LLS_ItemUnrealized;

            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 40); // TODO performance
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            Day testday = (Day)GetFirstVisibleItem(AgendaViewListbox);
            if (testday != null)
            {
                // Switch Highlighted Day
                Day d = App.ViewModel.CurrentTop;
                if (d != null)
                {
                    d.IsHighlighted = false;
                }
                App.ViewModel.CurrentTop = testday;
                testday.IsHighlighted = true;
            }

        }

        public object GetFirstVisibleItem(LongListSelector lls)
        {
            if (items.Count > 1)
            {
                var offset = FindViewport(lls).Viewport.Top;
                return (items.Where(x => Canvas.GetTop(x.Value) + x.Value.ActualHeight > offset)
                    .OrderBy(x => Canvas.GetTop(x.Value)).ToList())[1].Key;
            }
            else
                return null;

        }

        private void LLS_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind == LongListSelectorItemKind.Item)
            {
                object o = e.Container.DataContext;
                items[o] = e.Container;
            }
        }

        private void LLS_ItemUnrealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind == LongListSelectorItemKind.Item)
            {
                object o = e.Container.DataContext;
                items.Remove(o);
            }
        }

        private static ViewportControl FindViewport(DependencyObject parent)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childCount; i++)
            {
                var elt = VisualTreeHelper.GetChild(parent, i);
                if (elt is ViewportControl) return (ViewportControl)elt;
                var result = FindViewport(elt);
                if (result != null) return result;
            }
            return null;
        }
        #endregion

        #region Events

        public void onDayTap(object sender, System.Windows.Input.GestureEventArgs e)
        {


            //Open SingleDayView
            SingleDayView.Visibility = Visibility.Visible;
            VisualStateManager.GoToState(this, "OpenDelay", true);


            //Set selectedItem 
            var element = (FrameworkElement)sender;
            Day selectedItem = element.DataContext as Day;
            App.ViewModel.SingleDayViewModel.TappedDay = selectedItem;

            //if (apptWasTapped == false)
            //{
            //    SingleDayScrollViewer.ScrollToVerticalOffset(70 * 12);

            //}
            //if (firstTimeOpenSingleDayview == false)
            //{
            //    apptWasTapped = false;
            //}
        }

        private void scrollToDefaultOffset(object sender, EventArgs e)
        {
            //if (apptWasTapped == false)
            SingleDayScrollViewer.ScrollToVerticalOffset(70 * 12);

            //firstTimeOpenSingleDayview = false;

        }

        private void gridExit_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            closeDayView();
        }

        private void gridExit_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            closeDayView();
        }

        private void closeDayView()
        {
            //SingleDayView.Visibility = Visibility.Collapsed;
            VisualStateManager.GoToState(this, "Close", true);
        }




        public void ApptListItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PocalAppointment pocalAppointment = ((FrameworkElement)sender).DataContext as PocalAppointment;
            CalendarAPI.editAppointment(pocalAppointment);


        }

        private void SingleDay_Hourline_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HourLine hourLine = ((FrameworkElement)sender).DataContext as HourLine;
            var starttime = App.ViewModel.SingleDayViewModel.getStarTimeFromHourline(hourLine.Text);
            if (starttime != null)
            {
                DateTime dt = (DateTime)starttime;
                CalendarAPI.addAppointment(dt);
            }

        }


        #endregion

        private PocalAppointment aPA;
        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            aPA = ((FrameworkElement)sender).DataContext as PocalAppointment;
            SingleDayScrollViewer.ScrollToVerticalOffset(aPA.StartTime.Hour * 70 - 140);

        }

        void SingleDayViewModel_IsUpdated(object sender, EventArgs e)
        {

            if (aPA != null)
            {
                SingleDayScrollViewer.UpdateLayout();
                SingleDayScrollViewer.ScrollToVerticalOffset(aPA.StartTime.Hour * 70 - 140);
                App.ViewModel.SingleDayViewModel.TriggerScrollToOffset -= SingleDayViewModel_IsUpdated;
            }

        }


        private void Grid_Unloaded(object sender, RoutedEventArgs e)
        {
            PocalAppointment conmen = (sender as PocalAppointment);
            //conmen.ClearValue(FrameworkElement.DataContextProperty);
            ((FrameworkElement)sender).ClearValue(FrameworkElement.DataContextProperty);

        }


    }
}