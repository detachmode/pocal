//#define DEBUG_AGENT
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
using System.Threading;
using Pocal.Helper;
using System.Windows.Data;
using System.Windows.Media.Animation;
using Microsoft.Phone.Shell;
using Pocal.Resources;
using Microsoft.Phone.Scheduler;
using ScheduledTaskAgent1;
using Microsoft.Phone.Tasks;

namespace Pocal
{

    public partial class MainPage : PhoneApplicationPage
    {
        private Dictionary<object, ContentPresenter> realizedDayItems = new Dictionary<object, ContentPresenter>();

        private bool isPlayingOverviewAninmation = false;


        public MainPage()
        {
            App.LoadThemeRessources();
            loadStartup();

            App.ShowTrialPopUp();
        }



        private void loadStartup()
        {

            DataContext = App.ViewModel;
            InitializeComponent();

            AgendaViewAppbar();
            watchScrollingOfLLS();
            LiveTileManager.UpdateTileFromForeground();
            StartResourceIntensiveAgent();


        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
                return;
            }

            object navigationData = NavigationService.GetNavigationData();
            if (navigationData != null)
            {
                App.ViewModel.GoToDate((DateTime)navigationData);
            }
            else
                App.ViewModel.GoToDate(DateTime.Now);

        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (App.ViewModel.InModus == MainViewModel.Modi.OverView)
            {
                AgendaViewAppbar();
                leaveOverview();
            }

            //else
            //    scrollToToday();

            //e.Cancel = true;
            base.OnBackKeyPress(e);
        }


        #region ScheduledTask (LiveTile)

        PeriodicTask periodicTask;


        string periodicTaskName = "ResourceIntensiveAgent";

        private void StartResourceIntensiveAgent()
        {

            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule.
            if (periodicTask != null)
            {
                RemoveAgent(periodicTaskName);
            }

            periodicTask = new PeriodicTask(periodicTaskName);

            // The description is required for periodic agents. This is the string that the user
            // will see in the background services Settings page on the device.
            periodicTask.Description = "Updates Pocal LiveTile";

            // Place the call to Add in a try block in case the user has disabled agents.
            try
            {
                ScheduledActionService.Add(periodicTask);


                // If debugging is enabled, use LaunchForTest to launch the agent in one minute.
#if(DEBUG_AGENT)
                ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(10));
#endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    //MessageBox.Show("Background agents for this application have been disabled by the user.");
                }

            }
            catch (SchedulerServiceException)
            {
                // No user action required.

            }


        }

        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }


        #endregion

        #region AgendaView Code Behind

        private void AgendaViewAppbar()
        {

            ApplicationBar = new ApplicationBar();
            /*********** MENU ITEMS ***********/
            ApplicationBarMenuItem item1 = new ApplicationBarMenuItem();
            //item1.Text = "Einstellungen";
            //ApplicationBar.MenuItems.Add(item1);
            //ApplicationBarMenuItem item2 = new ApplicationBarMenuItem();
            //item2.Text = "Tutorial";
            //ApplicationBar.MenuItems.Add(item2);

            //ApplicationBarMenuItem item3 = new ApplicationBarMenuItem();
            //item3.Text = "Info";
            //ApplicationBar.MenuItems.Add(item3);

            /*********** BUTTONs ***********/
            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Images/back.png", UriKind.Relative);
            button1.Text = "Heute";
            ApplicationBar.Buttons.Add(button1);
            button1.Click += new EventHandler(delegate(object sender, EventArgs e)
            {
                scrollToToday();
            });

            /*********** ADD METHODE BUTTON ***********/
            ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Images/add.png", UriKind.Relative);
            button2.Text = "add";
            ApplicationBar.Buttons.Add(button2);
            button2.Click += new EventHandler(delegate(object sender, EventArgs e)
            {
                CalendarAPI.addAllDayAppointment(App.ViewModel.DayAtPointer.DT);

            });

            /*********** MONTHVIEW BUTTON ***********/
            ApplicationBarIconButton button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Images/feature.calendar.png", UriKind.Relative);
            button3.Text = "Gehe zu";
            ApplicationBar.Buttons.Add(button3);
            button3.Click += new EventHandler(delegate(object sender, EventArgs e)
            {
                //NavigationService.Navigate(new Uri("/MonthView.xaml", UriKind.Relative));
                openMonthView();

            });


            ApplicationBarIconButton button4 = new ApplicationBarIconButton();
            button4.IconUri = new Uri("/Images/bird.png", UriKind.Relative);
            button4.Text = "Overview";
            ApplicationBar.Buttons.Add(button4);
            button4.Click += new EventHandler(delegate(object sender, EventArgs e)
            {
                if (App.ViewModel.InModus == MainViewModel.Modi.AgendaView)
                {
                    toggleOverView();
                }
            });
        }

        private void watchScrollingOfLLS()
        {
            AgendaViewLLS.ItemRealized += LLS_EndOfList;
            AgendaViewLLS.ItemRealized += LLS_AddRealizedPocalAppointmentItem;
            AgendaViewLLS.ItemUnrealized += LLS_RemoveRealizedPocalAppointmentItem;


            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(checkDayAtCenterOfScreen_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 40);
            dispatcherTimer.Start();


            DispatcherTimer dispatcherTimer2 = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer2.Tick += new EventHandler(checkDayAtTopOfScreen_Tick);
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 0, 0, 40);
            dispatcherTimer2.Start();

            AgendaViewLLS.ManipulationStateChanged += AgendaScrolling_WhileSingleDayViewIsOpen_Fix;
        }

        private void checkDayAtTopOfScreen_Tick(object sender, EventArgs e)
        {
            Day topDay = (Day)getItemAtTopOfScreen(0);
            if (topDay != null)
            {
                int offset = 10;
                IEnumerable<Day> daysLoadedBeforeTopDay = App.ViewModel.Days.Where(x => x.DT < topDay.DT);

                int count = daysLoadedBeforeTopDay.Count();
                if (!App.ViewModel.IsCurrentlyLoading && count < offset)
                {
                    App.ViewModel.LoadIncrementalBackwards(7, DateTime.Now.Ticks);
                }


            }
        }

        private object getItemAtTopOfScreen(double offset)
        {
            if (realizedDayItems.Count > 1)
            {
                var LLS_Offset = FindViewport(AgendaViewLLS).Viewport.Top;
                LLS_Offset += offset;
                IEnumerable<KeyValuePair<object, ContentPresenter>> keyValuePairs = realizedDayItems.Where(x => Canvas.GetTop(x.Value) + x.Value.ActualHeight >= LLS_Offset);
                List<KeyValuePair<object, ContentPresenter>> keyValuePairsSorted = keyValuePairs.OrderBy(x => Canvas.GetTop(x.Value)).ToList();
                object obj = keyValuePairsSorted[0].Key;

                return obj;
            }
            else
                return null;
        }


        private void LLS_EndOfList(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind == LongListSelectorItemKind.Item)
            {
                Day day = e.Container.Content as Day;
                if (day != null)
                {
                    int offset = 10;
                    if (!App.ViewModel.IsCurrentlyLoading && App.ViewModel.Days.Count - App.ViewModel.Days.IndexOf(day) <= offset)
                    {
                        App.ViewModel.LoadIncrementalForward(7, DateTime.Now.Ticks);
                    }
                }
            }
        }


        private void LLS_AddRealizedPocalAppointmentItem(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind == LongListSelectorItemKind.Item)
            {
                object o = e.Container.DataContext;
                realizedDayItems[o] = e.Container;
            }
        }

        private void LLS_RemoveRealizedPocalAppointmentItem(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind == LongListSelectorItemKind.Item)
            {
                object o = e.Container.DataContext;
                realizedDayItems.Remove(o);
            }
        }

        private void AgendaScrolling_WhileSingleDayViewIsOpen_Fix(object sender, EventArgs e)
        {
            if (SingleDayView.Visibility == Visibility.Visible)
                closeSDV();
        }


        private void checkDayAtCenterOfScreen_Tick(object sender, EventArgs e)
        {
            double offset = 35;
            if (App.ViewModel.InModus == MainViewModel.Modi.OverView)
            {
                offset = offset * 2;
            }
            Day testday = (Day)getItemAtTopOfScreen(offset);
            if (testday != null)
            {
                Day d = App.ViewModel.DayAtPointer;
                if (d != null)
                {
                    d.IsHighlighted = false;
                }
                App.ViewModel.DayAtPointer = testday;
                testday.IsHighlighted = true;
            }

        }

        private object getItemAtCenterOfScreen()
        {
            if (realizedDayItems.Count > 1)
            {
                var LLS_Offset = getLLS_OffsetAtCenterOfScreen();

                IEnumerable<KeyValuePair<object, ContentPresenter>> keyValuePairs = realizedDayItems.Where(x => Canvas.GetTop(x.Value) + x.Value.ActualHeight >= LLS_Offset);
                List<KeyValuePair<object, ContentPresenter>> keyValuePairsSorted = keyValuePairs.OrderBy(x => Canvas.GetTop(x.Value)).ToList();
                if (keyValuePairsSorted.Count == 0)
                    return null;

                object obj = keyValuePairsSorted[0].Key;
                return obj;
            }
            else
                return null;

        }

        private double getLLS_OffsetAtCenterOfScreen()
        {
            var offset = FindViewport(AgendaViewLLS).Viewport.Top;
            offset += (730 / 2);
            return offset;
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


        private void scrollToToday()
        {
            App.ViewModel.GoToDate(DateTime.Now);

        }


        private void AgendaViewLLS_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            isPlayingOverviewAninmation = false;
        }


        private void AgendaViewLLS_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (isPlayingOverviewAninmation)
                return;

            if (Math.Abs(e.DeltaManipulation.Translation.X) > 10)
            {
                isPlayingOverviewAninmation = true;
                toggleOverView();

            }

        }


        private void toggleOverView()
        {
            if (App.ViewModel.InModus == MainViewModel.Modi.OverView)
                leaveOverview();
            else
                enterOverview();

        }

        private void LongList_Loaded(object sender, RoutedEventArgs e)
        {
            var sb = ((FrameworkElement)VisualTreeHelper.GetChild(AgendaViewLLS, 0)).FindName("VerticalScrollBar") as ScrollBar;
            sb.Margin = new Thickness(-10, 0, 0, 0);
            sb.Width = 0;

        }

        public void addBitmapCacheToSDV()
        {
            BitmapCache bmc = new BitmapCache() { RenderAtScale = 0.5 };
            SingleDayWindowBody.CacheMode = bmc;

        }

        public void removeBitmapCacheAfterAnimation()
        {
            Dispatcher.BeginInvoke(delegate
            {
                Thread.Sleep(200);
                SingleDayWindowBody.CacheMode = null;

            });

        }

        #endregion

        #region SingleDayView Code Behind

        public void SDVAppbar()
        {


            ApplicationBar = new ApplicationBar();
            /*********** MENU ITEMS ***********/
            ApplicationBarMenuItem item1 = new ApplicationBarMenuItem();
            //item1.Text = "Einstellungen";
            //ApplicationBar.MenuItems.Add(item1);
            //ApplicationBarMenuItem item2 = new ApplicationBarMenuItem();
            //item2.Text = "Tutorial";
            //ApplicationBar.MenuItems.Add(item2);

            //ApplicationBarMenuItem item3 = new ApplicationBarMenuItem();
            //item3.Text = "Info";
            //ApplicationBar.MenuItems.Add(item3);

            /*********** BUTTONs ***********/
            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Images/back.png", UriKind.Relative);
            button1.Text = "Heute";
            ApplicationBar.Buttons.Add(button1);
            button1.Click += new EventHandler(delegate(object sender, EventArgs e)
            {
                scrollToToday();
            });

            /*********** ADD METHODE BUTTON ***********/
            ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Images/add.png", UriKind.Relative);
            button2.Text = "add";
            ApplicationBar.Buttons.Add(button2);
            button2.Click += new EventHandler(delegate(object sender, EventArgs e)
            {
                CalendarAPI.addAllDayAppointment(App.ViewModel.SingleDayViewModel.TappedDay.DT);

            });

            /*********** MONTHVIEW BUTTON ***********/
            ApplicationBarIconButton button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Images/feature.calendar.png", UriKind.Relative);
            button3.Text = "Gehe zu";
            ApplicationBar.Buttons.Add(button3);
            button3.Click += new EventHandler(delegate(object sender, EventArgs e)
            {
                //NavigationService.Navigate(new Uri("/MonthView.xaml", UriKind.Relative));
                openMonthView();

            });
        }

        private void gridExit_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            closeSDV();
        }

        private void gridExit_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            closeSDV();
        }

        private void closeSDV()
        {
            VisualStateManager.GoToState(this, "Close", true);

            if (App.ViewModel.InModus == MainViewModel.Modi.OverViewSDV || App.ViewModel.InModus == MainViewModel.Modi.OverView)
            {
                OverviewAppbar();
                App.ViewModel.InModus = MainViewModel.Modi.OverView;
            }
            else
            {
                AgendaViewAppbar();
                App.ViewModel.InModus = MainViewModel.Modi.AgendaView;
            }

        }
        #endregion

        #region Overview Code Behind

        private List<ItemsControl> foundDayCards_ItemsControll;
        private List<StackPanel> foundStackPanels;


        private void OverviewAppbar()
        {


            ApplicationBar = new ApplicationBar();
            /*********** MENU ITEMS ***********/
            ApplicationBarMenuItem item1 = new ApplicationBarMenuItem();
            //item1.Text = "Einstellungen";
            //ApplicationBar.MenuItems.Add(item1);
            //ApplicationBarMenuItem item2 = new ApplicationBarMenuItem();
            //item2.Text = "Tutorial";
            //ApplicationBar.MenuItems.Add(item2);

            //ApplicationBarMenuItem item3 = new ApplicationBarMenuItem();
            //item3.Text = "Info";
            //ApplicationBar.MenuItems.Add(item3);

            /*********** BUTTONs ***********/
            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Images/back.png", UriKind.Relative);
            button1.Text = "Heute";
            ApplicationBar.Buttons.Add(button1);
            button1.Click += new EventHandler(delegate(object sender, EventArgs e)
            {
                scrollToToday();
            });

            /*********** ADD METHODE BUTTON ***********/
            ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Images/add.png", UriKind.Relative);
            button2.Text = "add";
            ApplicationBar.Buttons.Add(button2);
            button2.Click += new EventHandler(delegate(object sender, EventArgs e)
            {
                CalendarAPI.addAllDayAppointment(App.ViewModel.DayAtPointer.DT);

            });

            /*********** MONTHVIEW BUTTON ***********/
            ApplicationBarIconButton button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Images/feature.calendar.png", UriKind.Relative);
            button3.Text = "Gehe zu";
            ApplicationBar.Buttons.Add(button3);
            button3.Click += new EventHandler(delegate(object sender, EventArgs e)
            {
                //NavigationService.Navigate(new Uri("/MonthView.xaml", UriKind.Relative));
                openMonthView();

            });


            ApplicationBarIconButton button4 = new ApplicationBarIconButton();
            button4.IconUri = new Uri("/Images/cancel.png", UriKind.Relative);
            button4.Text = "Close Overview";
            ApplicationBar.Buttons.Add(button4);
            button4.Click += new EventHandler(delegate(object sender, EventArgs e)
            {
                if (App.ViewModel.InModus == MainViewModel.Modi.OverView)
                {
                    toggleOverView();
                }

                AgendaViewAppbar();
            });
        }

        private void enterOverview()
        {

            App.ViewModel.InModus = MainViewModel.Modi.OverView;
            Storyboard storyboard = this.Resources["EnterOverview"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }

            storyboard = this.Resources["AgendaPointerLong"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }

            foundDayCards_ItemsControll = new List<ItemsControl>();
            foundStackPanels = new List<StackPanel>();

            findItemControll(AgendaViewLLS);
            findItemStackPanelInItemsControll("DayCard_ApptItem");
            playStoryboardOfFoundStackPanels("EnterOverview");

            HeaderTitle.Text = "Overview";
            OverviewAppbar();



        }



        private void leaveOverview()
        {
            App.ViewModel.InModus = MainViewModel.Modi.AgendaView;

            Storyboard storyboard = this.Resources["LeaveOverview"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }

            storyboard = this.Resources["AgendaPointerShort"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }

            foundDayCards_ItemsControll = new List<ItemsControl>();
            foundStackPanels = new List<StackPanel>();

            findItemControll(AgendaViewLLS);
            findItemStackPanelInItemsControll("DayCard_ApptItem");

            playStoryboardOfFoundStackPanels("LeaveOverview");

            HeaderTitle.Text = "Agenda";
            AgendaViewAppbar();



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
                Storyboard storyboard = stackpanel.Resources[storyBoardKey] as Storyboard;
                if (storyboard != null)
                {
                    storyboard.Begin();
                }
            }

        }

        #endregion

        #region MonthView Code Behind

        private void MonthViewAppbar()
        {


            ApplicationBar = new ApplicationBar();

            /*********** BUTTONs ***********/
            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Images/back.png", UriKind.Relative);
            button1.Text = "Zurück";
            ApplicationBar.Buttons.Add(button1);
            button1.Click += new EventHandler(delegate(object sender, EventArgs e)
            {
                CloseMonthView();

            });


        }


        PivotItem pivotItem;
        MonthView_Control monthViewUserControl;

        private void openMonthView()
        {
            Canvas.SetZIndex(MonthView, 10);

            DateTime dt = App.ViewModel.DayAtPointer.DT;
            //addFPivotItemAndLoadAppointmentLines(dt);
            addPivotItem(dt);

            YearDisplay.Text = dt.Year.ToString();
            MonthViewAppbar();
            Dispatcher.BeginInvoke(delegate
            {
                addFourMorePivotItems(dt);

            });
        }

        public void CloseMonthView()
        {
            Canvas.SetZIndex(MonthView, -10);
            MonthsPivot.Items.Clear();

            switch (App.ViewModel.InModus)
            {
                case MainViewModel.Modi.AgendaView:
                    AgendaViewAppbar();
                    break;
                case MainViewModel.Modi.OverView:
                    OverviewAppbar();
                    break;
                case MainViewModel.Modi.OverViewSDV:
                    SDVAppbar();
                    break;
                case MainViewModel.Modi.AgendaViewSDV:
                    SDVAppbar();
                    break;
                default:
                    break;
            }

        }


        private void addFourMorePivotItems(DateTime lastAddedDate)
        {
            DateTime dt2 = lastAddedDate.AddMonths(1);
            DateTime dt3 = lastAddedDate.AddMonths(2);
            DateTime dt4 = lastAddedDate.AddMonths(3);
            DateTime dt5 = lastAddedDate.AddMonths(-1);


            addPivotItem(dt2);
            addPivotItem(dt3);
            addPivotItem(dt4);
            addPivotItem(dt5);




        }

        private void addPivotItem(DateTime dt)
        {
            monthViewUserControl = new MonthView_Control();
            monthViewUserControl.loadGridSetup(dt);

            pivotItem = new PivotItem();
            pivotItem.Content = monthViewUserControl;
            pivotItem.DataContext = dt;
            pivotItem.Margin = new Thickness(0, 0, 0, 0);
            pivotItem.Header = dt.ToString("MMMM");

            MonthsPivot.Items.Add(pivotItem);
        }



        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DependencyObject selectedPivotItem = ((Pivot)sender).ItemContainerGenerator.ContainerFromIndex(((Pivot)sender).SelectedIndex);
            if (selectedPivotItem == null)
                return;

            if (e.RemovedItems[0] == null)
            {
                return;
            }
            DateTime removedDateTime = (DateTime)(e.RemovedItems[0] as PivotItem).DataContext;
            DateTime addedDateTime = (DateTime)(e.AddedItems[0] as PivotItem).DataContext;

            if (removedDateTime < addedDateTime)
            {
                forwardPan(addedDateTime);
            }
            else
                backwardPan(addedDateTime);

            YearDisplay.Text = addedDateTime.Year.ToString();

        }

        private void forwardPan(DateTime addedDateTime)
        {
            PivotItem lastPivotItem = (PivotItem)getLastPivotItem();
            if (lastPivotItem == null)
            {
                return;
            }
            DateTime newDateTime = addedDateTime.AddMonths(3);

            MonthView_Control monthViewItem = new MonthView_Control();
            monthViewItem.loadGridSetup(newDateTime);
            lastPivotItem.DataContext = newDateTime;
            lastPivotItem.Content = monthViewItem;
            lastPivotItem.Header = newDateTime.ToString("MMMM");
        }

        private DependencyObject getLastPivotItem()
        {
            int index = ((Pivot)MonthsPivot).SelectedIndex;
            int lastIndex;

            lastIndex = (index + 3) % 5;

            DependencyObject lastPivotItem = ((Pivot)MonthsPivot).ItemContainerGenerator.ContainerFromIndex(lastIndex);
            return lastPivotItem;
        }



        private void backwardPan(DateTime addedDateTime)
        {

            PivotItem previousPivotItem = (PivotItem)getPreviousPivotItem();
            if (previousPivotItem == null)
            {
                return;
            }
            DateTime newDateTime = addedDateTime.AddMonths(-1);

            MonthView_Control monthViewItem = new MonthView_Control();
            monthViewItem.loadGridSetup(newDateTime);
            previousPivotItem.DataContext = newDateTime;
            previousPivotItem.Content = monthViewItem;
            previousPivotItem.Header = newDateTime.ToString("MMMM");
        }

        private DependencyObject getPreviousPivotItem()
        {
            int index = ((Pivot)MonthsPivot).SelectedIndex;
            int previousIndex;
            if (index == 0)
            {
                previousIndex = 4;
            }
            else
                previousIndex = index - 1;

            DependencyObject previousPivotItem = ((Pivot)MonthsPivot).ItemContainerGenerator.ContainerFromIndex(previousIndex);
            return previousPivotItem;
        }




        #endregion

    }
}