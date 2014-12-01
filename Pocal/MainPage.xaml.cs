﻿using System;
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
using System.Threading;
using Pocal.Helper;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace Pocal
{

    public partial class MainPage : PhoneApplicationPage
    {


        // Constructor
        public MainPage()
        {

            load();
            //CalendarAPI.AddTestAppointments();

        }

        private void load()
        {

            DataContext = App.ViewModel;
            InitializeComponent();

            SingleDayView.Opacity = 0.01;

            LoadDataFromSource();

            VisualStateManager.GoToState(this, "Close", true);

            AgendaViewListbox.ManipulationStateChanged += AgendaScrolling_WhileSingleDayViewIsOpen_Fix;
            AgendaViewListbox.ItemRealized += LLS_ItemRealized2;
            AgendaViewListbox.ItemRealized += LLS_ItemRealized;
            AgendaViewListbox.ItemUnrealized += LLS_ItemUnrealized2;
            watchPositionOfLongListSelector();
        }

        private async void LoadDataFromSource()
        {
            App.ViewModel.isCurrentlyLoading = true;
            if (App.ViewModel.Days.Count == 0)
            {
                DateTime dt = DateTime.Now.Date.AddDays(-10);

                await App.ViewModel.LoadPocalApptsAndDays(DateTime.Now, 10);
                await App.ViewModel.loadDatesOfThePast(3);

            }
            else
                await App.ViewModel.LoadPocalApptsAndDays(App.ViewModel.Days[App.ViewModel.Days.Count - 1].DT.AddDays(1), 20);

        }

        private void scrollToToday()
        {
            AgendaViewListbox.ScrollTo(App.ViewModel.Days[3]); 

        }


        #region AgendaView Events

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            scrollToToday();
            e.Cancel = true;
            base.OnBackKeyPress(e);
        }

        private void AgendaViewListbox_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (e.FinalVelocities.LinearVelocity.X > 0)
            {
                //enterOverview();
                switchMainPageView();
            }
            if (e.FinalVelocities.LinearVelocity.X < 0)
            {
                switchMainPageView();
                //leaveOverview();

            }
        }

        private void switchMainPageView()
        {
            if (App.ViewModel.InModus == MainViewModel.Modi.OverView)
                leaveOverview();
            else
                enterOverview();

        }

        private void LongList_Loaded(object sender, RoutedEventArgs e)
        {
            var sb = ((FrameworkElement)VisualTreeHelper.GetChild(AgendaViewListbox, 0)).FindName("VerticalScrollBar") as ScrollBar;
            sb.Margin = new Thickness(-10, 0, 0, 0);
            sb.Width = 0;

        }

        private void AgendaScrolling_WhileSingleDayViewIsOpen_Fix(object sender, EventArgs e)
        {
            if (SingleDayView.Visibility == Visibility.Visible)
                VisualStateManager.GoToState(this, "Close", true);

        }

        //Wird benötigt um die Position im Longlistselektor zu bestimmen um damit DeltaDays auszurechnen.
        private Dictionary<object, ContentPresenter> realizedPocalAppointmentItems = new Dictionary<object, ContentPresenter>();
        private void watchPositionOfLongListSelector()
        {
            //AgendaViewListbox.ItemRealized += LLS_ItemRealized;
            //AgendaViewListbox.ItemUnrealized += LLS_ItemUnrealized;

            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 40); // TODO performance
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            Day testday = (Day)GetFirstVisibleItem();
            if (testday != null)
            {
                // Switch Highlighted Day
                Day d = App.ViewModel.DayAtCenterOfScreen;
                if (d != null)
                {
                    d.IsHighlighted = false;
                }
                App.ViewModel.DayAtCenterOfScreen = testday;
                testday.IsHighlighted = true;
            }

        }

        public object GetFirstVisibleItem()
        {
            if (realizedPocalAppointmentItems.Count > 1)
            {
                var offset = FindViewport(AgendaViewListbox).Viewport.Top;
                if (App.ViewModel.InModus == MainViewModel.Modi.OverView)
                    offset += ((730 + 600) / 2);
                else
                    offset += (730 / 2);

                IEnumerable<KeyValuePair<object, ContentPresenter>> keyValuePairs = realizedPocalAppointmentItems.Where(x => Canvas.GetTop(x.Value) + x.Value.ActualHeight >= offset);
                List<KeyValuePair<object, ContentPresenter>> keyValuePairsSorted = keyValuePairs.OrderBy(x => Canvas.GetTop(x.Value)).ToList();
                object obj = keyValuePairsSorted[0].Key;

                return obj;
            }
            else
                return null;

        }

        private void LLS_ItemRealized2(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind == LongListSelectorItemKind.Item)
            {
                object o = e.Container.DataContext;
                realizedPocalAppointmentItems[o] = e.Container;
            }
        }

        private void LLS_ItemUnrealized2(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind == LongListSelectorItemKind.Item)
            {
                object o = e.Container.DataContext;
                realizedPocalAppointmentItems.Remove(o);
            }
        }





        private void LLS_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind == LongListSelectorItemKind.Item)
            {
                Day day = e.Container.Content as Day;
                if (day != null)
                {
                    int offset = 10;
                    // Only if there is no data that is currently getting loaded would be initiate the loading again
                    if (!App.ViewModel.isCurrentlyLoading && App.ViewModel.Days.Count - App.ViewModel.Days.IndexOf(day) <= offset)
                    {
                        LoadDataFromSource();
                        //MessageBox.Show("endOfList");
                    }
                }
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

        #region Exit/Closing Events


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

            VisualStateManager.GoToState(this, "Close", true);
        }



        #endregion

        #region SDV Events

        public void SDV_AppointmentTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PocalAppointment pocalAppointment = ((FrameworkElement)sender).DataContext as PocalAppointment;
            CalendarAPI.editAppointment(pocalAppointment);


        }

        private void SDV_Hourline_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HourLine hourLine = ((FrameworkElement)sender).DataContext as HourLine;
            var starttime = App.ViewModel.SingleDayViewModel.getStarTimeFromHourline(hourLine.Text);
            if (starttime != null)
            {
                DateTime dt = (DateTime)starttime;
                CalendarAPI.addAppointment(dt);
            }

        }

        private void SingleDayScrollViewer_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (e.FinalVelocities.LinearVelocity.X > 0)
            {
                changeTappedDay(-1);
            }
            if (e.FinalVelocities.LinearVelocity.X < 0)
            {
                changeTappedDay(+1);
            }
        }

        private static void changeTappedDay(int add)
        {
            DateTime nextDate = App.ViewModel.SingleDayViewModel.TappedDay.DT.AddDays(add);
            Day newTappedDay = App.ViewModel.Days.FirstOrDefault(x => x.DT.Date == nextDate.Date);
            if (newTappedDay != null)
                App.ViewModel.SingleDayViewModel.TappedDay = newTappedDay;
            App.ViewModel.ConflictManager.solveConflicts();
        }

        #endregion

        #region ViewSwitcher Events


        private void DayCard_ApptTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ViewSwitcher.setScrollToPa(((FrameworkElement)sender).DataContext as PocalAppointment);
            ViewSwitcher.from = ViewSwitcher.Sender.ApptTap;

        }

        private void addBitmapCacheToSDV()
        {

            SingleDayWindowBody.CacheMode = new BitmapCache() { RenderAtScale = 0.6 };
        }

        private void removeBitmapCacheAfterAnimation()
        {
            Dispatcher.BeginInvoke(delegate
            {
                Thread.Sleep(400);
                SingleDayWindowBody.CacheMode = null;

            });
        }

        private void DayCard_HeaderTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            addBitmapCacheToSDV();
            ViewSwitcher.from = ViewSwitcher.Sender.HeaderTap;
            removeBitmapCacheAfterAnimation();
        }


        private void OpenSdvAndSetTappedDay(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (App.ViewModel.InModus == MainViewModel.Modi.OverView)
                leaveOverview();
            else
                ViewSwitcher.SwitchToSDV(sender);

        }
        #endregion

        #region Play Enter / Leave Overview Storyboard Animation

        private List<ItemsControl> foundDayCards_ItemsControll;
        private List<StackPanel> foundStackPanels;

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            switchMainPageView();
        }

        private void enterOverview()
        {
            App.ViewModel.InModus = MainViewModel.Modi.OverView;
            Storyboard storyboard = AgendaViewBody.Resources["EnterOverview"] as Storyboard;
            storyboard.Begin();

            foundDayCards_ItemsControll = new List<ItemsControl>();
            foundStackPanels = new List<StackPanel>();

            findItemControll(AgendaViewListbox);
            findItemStackPanelInItemsControll("DayCard_ApptItem");
            playStoryboardOfFoundStackPanels("EnterOverview");

            playStoryboardOfAgendaPointer("EnterOverview");


        }

        private void playStoryboardOfAgendaPointer(string storyBoardKey)
        {
            Storyboard storyboard2 = AgendaPointer.Resources[storyBoardKey] as Storyboard;
            storyboard2.Begin();
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            //leaveOverview();


        }

        private void leaveOverview()
        {
            App.ViewModel.InModus = MainViewModel.Modi.AgendaView;

            Storyboard storyboard = AgendaViewBody.Resources["LeaveOverview"] as Storyboard;
            storyboard.Begin();

            foundDayCards_ItemsControll = new List<ItemsControl>();
            foundStackPanels = new List<StackPanel>();

            findItemControll(AgendaViewListbox);
            findItemStackPanelInItemsControll("DayCard_ApptItem");
            playStoryboardOfFoundStackPanels("LeaveOverview");

            playStoryboardOfAgendaPointer("LeaveOverview");

        }


        private void findItemControll(DependencyObject targeted_control)
        {

            var count = VisualTreeHelper.GetChildrenCount(targeted_control);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(targeted_control, i);
                    var test = child.GetType();
                    if (child is ItemsControl)
                    {
                        foundDayCards_ItemsControll.Add((ItemsControl)child);
                    }
                    else
                    {
                        findItemControll(child);
                    }
                }
            }
            return;
        }

        private void findItemStackPanelInItemsControll(string stackPanelName)
        {
            for (int j = 0; j < foundDayCards_ItemsControll.Count; j++)
            {
                for (int i = 0; i < foundDayCards_ItemsControll[j].Items.Count; i++)
                {
                    DependencyObject d = foundDayCards_ItemsControll[j].ItemContainerGenerator.ContainerFromIndex(i);
                    findStackPanels(d, stackPanelName);
                }
            }
        }


        private void findStackPanels(DependencyObject targeted_control, string stackPanelName)
        {
            var count = VisualTreeHelper.GetChildrenCount(targeted_control);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(targeted_control, i);
                    if (child is StackPanel)
                    {
                        StackPanel stackpanel = (StackPanel)child;
                        if (stackpanel.Name == stackPanelName)
                        {
                            foundStackPanels.Add(stackpanel);
                        }
                    }
                    else
                    {
                        findStackPanels(child, stackPanelName);
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void playStoryboardOfFoundStackPanels(string storyBoardKey)
        {
            foreach (var stackpanel in foundStackPanels)
            {
                Storyboard StyBrd = stackpanel.Resources[storyBoardKey] as Storyboard;
                StyBrd.Begin();
            }

        }

        #endregion



    }
}