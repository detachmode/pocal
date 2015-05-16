//#define DEBUG_AGENT

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Pocal.Helper;
using Pocal.Resources;
using Pocal.ViewModel;
using ScheduledTaskAgent1;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

#pragma warning disable 4014

namespace Pocal
{
    public partial class MainPage
    {
        private readonly Dictionary<object, ContentPresenter> _realizedDayItems =
            new Dictionary<object, ContentPresenter>();

        private bool _isPlayingOverviewAninmation;

        public MainPage()
        {
            App.LoadMyRessources();
            LoadStartup();

            App.ShowTrialPopUp();
        }

        private void LoadStartup()
        {
            DataContext = App.ViewModel;
            InitializeComponent();

            AgendaViewAppbar();
            WatchScrollingOfLls();
            LiveTileManager.UpdateTileFromForeground();
            StartResourceIntensiveAgent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }

            var navigationData = NavigationService.GetNavigationData();
            if (navigationData != null)
            {
                App.ViewModel.GoToDate((DateTime) navigationData);
            }
            else
                App.ViewModel.GoToDate(DateTime.Now);
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (App.ViewModel.InModus == MainViewModel.Modi.OverView)
            {
                AgendaViewAppbar();
                LeaveOverview();
                e.Cancel = true;
            }
            if (App.ViewModel.InModus == MainViewModel.Modi.MonthView)
            {
                CloseMonthView();
                e.Cancel = true;
            }


            base.OnBackKeyPress(e);
        }

        #region ScheduledTask (LiveTile)

        private PeriodicTask _periodicTask;


        private readonly string _periodicTaskName = "ResourceIntensiveAgent";

        private void StartResourceIntensiveAgent()
        {
            _periodicTask = ScheduledActionService.Find(_periodicTaskName) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule.
            if (_periodicTask != null)
            {
                RemoveAgent(_periodicTaskName);
            }

            _periodicTask = new PeriodicTask(_periodicTaskName);

            // The description is required for periodic agents. This is the string that the user
            // will see in the background services Settings page on the device.
            _periodicTask.Description = "Updates Pocal LiveTile";

            // Place the call to Add in a try block in case the user has disabled agents.
            try
            {
                ScheduledActionService.Add(_periodicTask);


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

        private static void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion

        #region AgendaView Code Behind

        private void AgendaViewAppbar()
        {
            ApplicationBar = new ApplicationBar();
            /*********** MENU ITEMS ***********/
            var item1 = new ApplicationBarMenuItem();
            item1.Text = AppResources.SettingsPageTitle;
            item1.Click += delegate { NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative)); };


            ApplicationBar.MenuItems.Add(item1);

            var item3 = new ApplicationBarMenuItem();
            item3.Text = "Info";
            item3.Click += item3_Click;
            ApplicationBar.MenuItems.Add(item3);


            /*********** BUTTONs ***********/
            var button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Images/back.png", UriKind.Relative);
            button1.Text = AppResources.AppBarButtonToday;
            ApplicationBar.Buttons.Add(button1);
            button1.Click += delegate { ScrollToToday(); };

            /*********** ADD METHODE BUTTON ***********/
            var button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Images/add.png", UriKind.Relative);
            button2.Text = AppResources.AppBarAdd;
            ApplicationBar.Buttons.Add(button2);
            button2.Click += delegate { PocalAppointmentHelper.AddAllDayAppointment(App.ViewModel.DayAtPointer.Dt); };

            /*********** MONTHVIEW BUTTON ***********/
            var button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Images/feature.calendar.png", UriKind.Relative);
            button3.Text = AppResources.AppBarGoTo;
            ApplicationBar.Buttons.Add(button3);
            button3.Click += delegate
            {
                //NavigationService.Navigate(new Uri("/MonthView.xaml", UriKind.Relative));
                OpenMonthView();
            };


            var button4 = new ApplicationBarIconButton();
            button4.IconUri = new Uri("/Images/bird.png", UriKind.Relative);
            button4.Text = "Search";
            ApplicationBar.Buttons.Add(button4);
            button4.Click += delegate
            {
                //if (App.ViewModel.InModus == MainViewModel.Modi.AgendaView)
                //{
                //    ToggleOverView();
                //}
                OpenSearchView();
            };
        }

        private void item3_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/InfoPage.xaml", UriKind.Relative));
        }

        private void WatchScrollingOfLls()
        {
            AgendaViewLls.ItemRealized += LLS_EndOfList;
            AgendaViewLls.ItemRealized += LLS_AddRealizedPocalAppointmentItem;
            AgendaViewLls.ItemUnrealized += LLS_RemoveRealizedPocalAppointmentItem;


            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += checkDayAtCenterOfScreen_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 40);
            dispatcherTimer.Start();


            var dispatcherTimer2 = new DispatcherTimer();
            dispatcherTimer2.Tick += checkDayAtTopOfScreen_Tick;
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 0, 0, 40);
            dispatcherTimer2.Start();

            AgendaViewLls.ManipulationStateChanged += AgendaScrolling_WhileSingleDayViewIsOpen_Fix;
        }

        private void checkDayAtTopOfScreen_Tick(object sender, EventArgs e)
        {
            var topDay = (Day) GetItemAtTopOfScreen(0);          
            if (topDay == null) return;

            const int offset = 10;
            var daysLoadedBeforeTopDay = App.ViewModel.Days.Where(x => x.Dt < topDay.Dt);

            var count = daysLoadedBeforeTopDay.Count();
            if (!App.ViewModel.IsCurrentlyLoading && count < offset)
            {
                App.ViewModel.LoadIncrementalBackwards(7, DateTime.Now.Ticks);
            }
        }

        private object GetItemAtTopOfScreen(double offset)
        {
            if (_realizedDayItems.Count <= 1) return null;

            double llsOffset;
            llsOffset = FindViewport(AgendaViewLls).Viewport.Top;
            llsOffset += offset;

            var keyValuePairs = _realizedDayItems.Where(x => Canvas.GetTop(x.Value) + x.Value.ActualHeight >= llsOffset);
            var keyValuePairsSorted = keyValuePairs.OrderBy(x => Canvas.GetTop(x.Value)).ToList();
            var obj = keyValuePairsSorted[0].Key;

            return obj;
        }


        private void LLS_EndOfList(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
           
            var day = e.Container.Content as Day;
            if (day == null) return;    
    
            if (App.ViewModel.IsCurrentlyLoading) return;
           
            const int offset = 10;
            if (App.ViewModel.Days.Count - App.ViewModel.Days.IndexOf(day) <= offset)
            {
                App.ViewModel.LoadIncrementalForward(7, DateTime.Now.Ticks);
            }
        }


        private void LLS_AddRealizedPocalAppointmentItem(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var o = e.Container.DataContext;
            _realizedDayItems[o] = e.Container;
        }

        private void LLS_RemoveRealizedPocalAppointmentItem(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var o = e.Container.DataContext;
            _realizedDayItems.Remove(o);
        }

        private void AgendaScrolling_WhileSingleDayViewIsOpen_Fix(object sender, EventArgs e)
        {
            if (SingleDayView.Visibility == Visibility.Visible)
                CloseSdv();
        }


        private void checkDayAtCenterOfScreen_Tick(object sender, EventArgs e)
        {
            double offset = 35;
            if (App.ViewModel.InModus == MainViewModel.Modi.OverView)
            {
                offset = offset*2;
            }
            var testday = (Day) GetItemAtTopOfScreen(offset);
            if (testday == null) return;

            var d = App.ViewModel.DayAtPointer;
            if (d != null)
            {
                d.IsHighlighted = false;
            }

            App.ViewModel.DayAtPointer = testday;
            testday.IsHighlighted = true;
        }


        private static ViewportControl FindViewport(DependencyObject parent)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childCount; i++)
            {
                var elt = VisualTreeHelper.GetChild(parent, i);
                
                var control = elt as ViewportControl;
                if (control != null) return control;
               
                var result = FindViewport(elt);
                if (result != null) return result;
            }
            return null;
        }


        private static void ScrollToToday()
        {
            App.ViewModel.GoToDate(DateTime.Now);
        }


        private void AgendaViewLLS_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            _isPlayingOverviewAninmation = false;
        }


        private void AgendaViewLLS_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (_isPlayingOverviewAninmation)
                return;

            if (Math.Abs(e.DeltaManipulation.Translation.X) > 10)
            {
                _isPlayingOverviewAninmation = true;
                ToggleOverView();
            }
        }


        private void ToggleOverView()
        {
            if (App.ViewModel.InModus == MainViewModel.Modi.OverView)
                LeaveOverview();
            else
                EnterOverview();
        }

        private void LongList_Loaded(object sender, RoutedEventArgs e)
        {
            var sb =
                ((FrameworkElement) VisualTreeHelper.GetChild(AgendaViewLls, 0)).FindName("VerticalScrollBar") as
                    ScrollBar;
            if (sb == null) return;
            sb.Margin = new Thickness(-10, 0, 0, 0);
            sb.Width = 0;
        }

        public void AddBitmapCacheToSdv()
        {
            var bmc = new BitmapCache {RenderAtScale = 0.5};
            SingleDayWindowBody.CacheMode = bmc;
        }

        public void RemoveBitmapCacheAfterAnimation()
        {
            Dispatcher.BeginInvoke(delegate
            {
                Thread.Sleep(200);
                SingleDayWindowBody.CacheMode = null;
            });
        }

        #endregion

        #region SingleDayView Code Behind

        public void SdvAppbar()
        {
            ApplicationBar = new ApplicationBar();
            /*********** MENU ITEMS ***********/
            //ApplicationBarMenuItem item1 = new ApplicationBarMenuItem();
            //item1.Text = AppResources.SettingsPageTitle;
            //ApplicationBar.MenuItems.Add(item1);
            //ApplicationBarMenuItem item2 = new ApplicationBarMenuItem();
            //item2.Text = "Tutorial";
            //ApplicationBar.MenuItems.Add(item2);

            var item3 = new ApplicationBarMenuItem();
            item3.Text = "Info";
            item3.Click += item3_Click;
            ApplicationBar.MenuItems.Add(item3);

            /*********** BUTTONs ***********/
            var button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Images/back.png", UriKind.Relative);
            button1.Text = AppResources.AppBarButtonToday;
            ApplicationBar.Buttons.Add(button1);
            button1.Click += delegate { ScrollToToday(); };

            /*********** ADD METHODE BUTTON ***********/
            var button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Images/add.png", UriKind.Relative);
            button2.Text = AppResources.AppBarAdd;
            ApplicationBar.Buttons.Add(button2);
            button2.Click +=
                delegate { PocalAppointmentHelper.AddAllDayAppointment(App.ViewModel.SingleDayViewModel.TappedDay.Dt); };

            /*********** MONTHVIEW BUTTON ***********/
            var button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Images/feature.calendar.png", UriKind.Relative);
            button3.Text = AppResources.AppBarGoTo;
            ApplicationBar.Buttons.Add(button3);
            button3.Click += delegate
            {
                //NavigationService.Navigate(new Uri("/MonthView.xaml", UriKind.Relative));
                OpenMonthView();
            };
        }

        private void gridExit_MouseMove(object sender, MouseEventArgs e)
        {
            CloseSdv();
        }

        private void gridExit_Tap(object sender, GestureEventArgs e)
        {
            CloseSdv();
        }

        private void CloseSdv()
        {
            VisualStateManager.GoToState(this, "Close", true);

            if (App.ViewModel.InModus == MainViewModel.Modi.OverViewSdv ||
                App.ViewModel.InModus == MainViewModel.Modi.OverView)
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

        private List<ItemsControl> _foundDayCardsItemsControll;
        private List<StackPanel> _foundStackPanels;


        private void OverviewAppbar()
        {
            ApplicationBar = new ApplicationBar();
            /*********** MENU ITEMS ***********/
            var item1 = new ApplicationBarMenuItem();
            item1.Text = AppResources.SettingsPageTitle;
            item1.Click += delegate { NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative)); };


            ApplicationBar.MenuItems.Add(item1);

            var item3 = new ApplicationBarMenuItem();
            item3.Text = "Info";
            item3.Click += item3_Click;
            ApplicationBar.MenuItems.Add(item3);

            /*********** BUTTONs ***********/
            var button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Images/back.png", UriKind.Relative);
            button1.Text = AppResources.AppBarButtonToday;
            ApplicationBar.Buttons.Add(button1);
            button1.Click += delegate { ScrollToToday(); };

            /*********** ADD METHODE BUTTON ***********/
            var button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Images/add.png", UriKind.Relative);
            button2.Text = AppResources.AppBarAdd;
            ApplicationBar.Buttons.Add(button2);
            button2.Click += delegate { PocalAppointmentHelper.AddAllDayAppointment(App.ViewModel.DayAtPointer.Dt); };

            /*********** MONTHVIEW BUTTON ***********/
            var button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Images/feature.calendar.png", UriKind.Relative);
            button3.Text = AppResources.AppBarGoTo;
            ApplicationBar.Buttons.Add(button3);
            button3.Click += delegate
            {
                //NavigationService.Navigate(new Uri("/MonthView.xaml", UriKind.Relative));
                OpenMonthView();
            };


            var button4 = new ApplicationBarIconButton();
            button4.IconUri = new Uri("/Images/cancel.png", UriKind.Relative);
            button4.Text = AppResources.AppBarCloseOverview;
            ApplicationBar.Buttons.Add(button4);
            button4.Click += delegate
            {
                if (App.ViewModel.InModus == MainViewModel.Modi.OverView)
                {
                    ToggleOverView();
                }

                AgendaViewAppbar();
            };
        }

        private void EnterOverview()
        {
            App.ViewModel.InModus = MainViewModel.Modi.OverView;
            var storyboard = Resources["EnterOverview"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }

            storyboard = Resources["AgendaPointerLong"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }

            _foundDayCardsItemsControll = new List<ItemsControl>();
            _foundStackPanels = new List<StackPanel>();

            FindItemControll(AgendaViewLls);
            FindItemStackPanelInItemsControll("DayCard_ApptItem");
            PlayStoryboardOfFoundStackPanels("EnterOverview");

            HeaderTitle.Text = "Overview";
            OverviewAppbar();
        }


        private void LeaveOverview()
        {
            App.ViewModel.InModus = MainViewModel.Modi.AgendaView;

            var storyboard = Resources["LeaveOverview"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }

            storyboard = Resources["AgendaPointerShort"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }

            _foundDayCardsItemsControll = new List<ItemsControl>();
            _foundStackPanels = new List<StackPanel>();

            FindItemControll(AgendaViewLls);
            FindItemStackPanelInItemsControll("DayCard_ApptItem");

            PlayStoryboardOfFoundStackPanels("LeaveOverview");

            HeaderTitle.Text = "Agenda";
            AgendaViewAppbar();
        }


        private void FindItemControll(DependencyObject targetedControl)
        {
            var count = VisualTreeHelper.GetChildrenCount(targetedControl);
            if (count <= 0) return;

            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(targetedControl, i);
                var item = child as ItemsControl;
                if (item != null)
                {
                    _foundDayCardsItemsControll.Add(item);
                }
                else
                {
                    FindItemControll(child);
                }
            }
        }

        private void FindItemStackPanelInItemsControll(string stackPanelName)
        {
            foreach (var itemsControl in _foundDayCardsItemsControll)
            {
                for (var i = 0; i < itemsControl.Items.Count; i++)
                {
                    var d = itemsControl.ItemContainerGenerator.ContainerFromIndex(i);
                    FindStackPanels(d, stackPanelName);
                }
            }
        }


        private void FindStackPanels(DependencyObject targetedControl, string stackPanelName)
        {
            var count = VisualTreeHelper.GetChildrenCount(targetedControl);
            if (count <= 0) return;

            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(targetedControl, i);
                var stackPanel = child as StackPanel;
                if (stackPanel != null)
                {
                    var stackpanel = stackPanel;
                    if (stackpanel.Name == stackPanelName)
                    {
                        _foundStackPanels.Add(stackpanel);
                    }
                }
                else
                {
                    FindStackPanels(child, stackPanelName);
                }
            }
        }

        private void PlayStoryboardOfFoundStackPanels(string storyBoardKey)
        {
            foreach (var stackpanel in _foundStackPanels)
            {
                var storyboard = stackpanel.Resources[storyBoardKey] as Storyboard;
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
            var button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Images/back.png", UriKind.Relative);
            button1.Text = AppResources.AppBarBack;
            ApplicationBar.Buttons.Add(button1);
            button1.Click += delegate { CloseMonthView(); };
        }


        private PivotItem _pivotItem;
        private MonthView_Control _monthViewUserControl;

        private void OpenMonthView()
        {
            App.ViewModel.ModusBefore = App.ViewModel.InModus;
            App.ViewModel.InModus = MainViewModel.Modi.MonthView;
            Canvas.SetZIndex(MonthView, 10);

            var dt = App.ViewModel.DayAtPointer.Dt;
            //addFPivotItemAndLoadAppointmentLines(dt);
            AddPivotItem(dt);

            YearDisplay.Text = dt.Year.ToString();
            MonthViewAppbar();
            Dispatcher.BeginInvoke(delegate { AddFourMorePivotItems(dt); });
        }

        public void CloseMonthView()
        {
            App.ViewModel.InModus = App.ViewModel.ModusBefore;
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
                case MainViewModel.Modi.OverViewSdv:
                    SdvAppbar();
                    break;
                case MainViewModel.Modi.AgendaViewSdv:
                    SdvAppbar();
                    break;
            }
        }


        private void AddFourMorePivotItems(DateTime lastAddedDate)
        {
            var dt2 = lastAddedDate.AddMonths(1);
            var dt3 = lastAddedDate.AddMonths(2);
            var dt4 = lastAddedDate.AddMonths(3);
            var dt5 = lastAddedDate.AddMonths(-1);


            AddPivotItem(dt2);
            AddPivotItem(dt3);
            AddPivotItem(dt4);
            AddPivotItem(dt5);
        }

        private void AddPivotItem(DateTime dt)
        {
            _monthViewUserControl = new MonthView_Control();
            _monthViewUserControl.LoadGridSetup(dt);

            _pivotItem = new PivotItem();
            _pivotItem.Content = _monthViewUserControl;
            _pivotItem.DataContext = dt;
            _pivotItem.Margin = new Thickness(0, 0, 0, 0);
            _pivotItem.Header = dt.ToString("MMMM");

            MonthsPivot.Items.Add(_pivotItem);
        }


        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPivotItem =
                ((Pivot) sender).ItemContainerGenerator.ContainerFromIndex(((Pivot) sender).SelectedIndex);
            if (selectedPivotItem == null)
                return;

            var pivotItem = e.RemovedItems[0] as PivotItem;
            if (pivotItem == null) return;
            var removedDateTime = (DateTime) pivotItem.DataContext;

            var item = e.AddedItems[0] as PivotItem;
            if (item == null) return;
            var addedDateTime = (DateTime) item.DataContext;

            if (removedDateTime < addedDateTime)
            {
                ForwardPan(addedDateTime);
            }
            else
                BackwardPan(addedDateTime);

            YearDisplay.Text = addedDateTime.Year.ToString();
        }

        private void ForwardPan(DateTime addedDateTime)
        {
            var lastPivotItem = (PivotItem) GetLastPivotItem();
            if (lastPivotItem == null)
            {
                return;
            }
            var newDateTime = addedDateTime.AddMonths(3);

            var monthViewItem = new MonthView_Control();
            monthViewItem.LoadGridSetup(newDateTime);
            lastPivotItem.DataContext = newDateTime;
            lastPivotItem.Content = monthViewItem;
            lastPivotItem.Header = newDateTime.ToString("MMMM");
        }

        private DependencyObject GetLastPivotItem()
        {
            var index = MonthsPivot.SelectedIndex;
            int lastIndex;

            lastIndex = (index + 3)%5;

            var lastPivotItem = MonthsPivot.ItemContainerGenerator.ContainerFromIndex(lastIndex);
            return lastPivotItem;
        }


        private void BackwardPan(DateTime addedDateTime)
        {
            var previousPivotItem = (PivotItem) GetPreviousPivotItem();
            if (previousPivotItem == null)
            {
                return;
            }
            var newDateTime = addedDateTime.AddMonths(-1);

            var monthViewItem = new MonthView_Control();
            monthViewItem.LoadGridSetup(newDateTime);
            previousPivotItem.DataContext = newDateTime;
            previousPivotItem.Content = monthViewItem;
            previousPivotItem.Header = newDateTime.ToString("MMMM");
        }

        private DependencyObject GetPreviousPivotItem()
        {
            var index = MonthsPivot.SelectedIndex;
            int previousIndex;
            if (index == 0)
            {
                previousIndex = 4;
            }
            else
                previousIndex = index - 1;

            var previousPivotItem = MonthsPivot.ItemContainerGenerator.ContainerFromIndex(previousIndex);
            return previousPivotItem;
        }

        #endregion


        #region Search Code Behind

        private void SearchViewAppbar()
        {
            ApplicationBar = new ApplicationBar();

            /*********** BUTTONs ***********/
            var button1 = new ApplicationBarIconButton
            {
                IconUri = new Uri("/Images/back.png", UriKind.Relative),               
                Text = AppResources.AppBarBack

            };

            ApplicationBar.Buttons.Add(button1);
            button1.Click += delegate { CloseSearchView(); };
        }

        private void OpenSearchView()
        {
            App.ViewModel.ModusBefore = App.ViewModel.InModus;
            App.ViewModel.InModus = MainViewModel.Modi.SearchView;
           
            Canvas.SetZIndex(SearchGrid, 10);
            TheSearchControl.OpenSearchControl();
            SearchViewAppbar();

        }

        public void CloseSearchView()
        {
            App.ViewModel.InModus = App.ViewModel.ModusBefore;
            Canvas.SetZIndex(SearchGrid, -11);
            //SearchGrid.Items.Clear();

            switch (App.ViewModel.InModus)
            {
                case MainViewModel.Modi.AgendaView:
                    AgendaViewAppbar();
                    break;
                case MainViewModel.Modi.OverView:
                    OverviewAppbar();
                    break;
                case MainViewModel.Modi.OverViewSdv:
                    SdvAppbar();
                    break;
                case MainViewModel.Modi.AgendaViewSdv:
                    SdvAppbar();
                    break;
            }
        }
        #endregion

    }
}