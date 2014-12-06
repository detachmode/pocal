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

namespace Pocal
{

    public partial class MainPage : PhoneApplicationPage
    {
        private Dictionary<object, ContentPresenter> realizedDayItems = new Dictionary<object, ContentPresenter>();

        // Constructor
        public MainPage()
        {

            loadStartup();
            //CalendarAPI.AddTestAppointments();

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
                return;
            }
            
            string goToDate = "";
            if (NavigationContext.QueryString.TryGetValue("GoToDate", out goToDate))
            {
                App.ViewModel.GoToDate(DateTime.Now);
            }
            else
                App.ViewModel.GoToDate(DateTime.Now);

        }

        private void loadStartup()
        {

            DataContext = App.ViewModel;
            InitializeComponent();

            //fixme
            //SingleDayView.Opacity = 0.01;

            //App.ViewModel.GoToDate(DateTime.Now);
            //VisualStateManager.GoToState(this, "Close", true);

            DefaultAppbar();
            watchScrollingOfLLS();


        }

        #region watch Scrolling of AgendaViewLLS
        private void watchScrollingOfLLS()
        {

            AgendaViewLLS.ItemRealized += LLS_EndOfList;
            AgendaViewLLS.ItemRealized += LLS_AddRealizedPocalAppointmentItem;
            AgendaViewLLS.ItemUnrealized += LLS_RemoveRealizedPocalAppointmentItem;


            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(checkDayAtCenterOfScreen_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 40);
            dispatcherTimer.Start();

            // FIXME Performance / Naming
            DispatcherTimer dispatcherTimer2 = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer2.Tick += new EventHandler(checkDayATTopOfScreen_Tick); // TODO Performance   !!!!!!!!!
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 0, 0, 40);
            dispatcherTimer2.Start();

            AgendaViewLLS.ManipulationStateChanged += AgendaScrolling_WhileSingleDayViewIsOpen_Fix;
        }

        private void checkDayATTopOfScreen_Tick(object sender, EventArgs e)
        {
            //Day centerDay = (Day)getItemAtCenterOfScreen();
            Day topDay = (Day)getItemAtTopOfScreen();
            if (topDay != null)
            {
                // if topDay == Item
                int offset = 10;
                IEnumerable<Day> daysLoadedBeforeTopDay = App.ViewModel.Days.Where(x => x.DT < topDay.DT);
                //List<KeyValuePair<object, ContentPresenter>> keyValuePairsSorted = daysLoadedBeforeTopDay.OrderBy(x => Canvas.GetTop(x.Value)).ToList();
                int count = daysLoadedBeforeTopDay.Count();
                if (!App.ViewModel.IsCurrentlyLoading && count < offset)
                {
                    App.ViewModel.LoadDaysOfPastAsync(7);
                }

                //App.ViewModel.DayAtCenterOfScreen = topDay;

            }
        }

        private object getItemAtTopOfScreen()
        {
            if (realizedDayItems.Count > 1)
            {
                var LLS_Offset = FindViewport(AgendaViewLLS).Viewport.Top;
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
                        App.ViewModel.LoadMoreDays(7);
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
                VisualStateManager.GoToState(this, "Close", true);

        }



        private void checkDayAtCenterOfScreen_Tick(object sender, EventArgs e)
        {

            Day testday = (Day)getItemAtCenterOfScreen();
            if (testday != null)
            {
                // Switch Highlighted Day
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
            //if (App.ViewModel.InModus == MainViewModel.Modi.OverView)
            //    offset += ((730 + 600) / 2);
            //else
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

        #endregion



        #region AgendaView Events

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            scrollToToday();
            e.Cancel = true;
            base.OnBackKeyPress(e);
        }

        private void scrollToToday()
        {
            App.ViewModel.GoToDate(DateTime.Now);
            //Day today = App.ViewModel.dispDays[3];
            //AgendaViewLLS.ScrollTo(today);

        }


        private void toggleOverView_Gesture(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (e.FinalVelocities.LinearVelocity.X > 0)
            {
                toggleOverView();
            }
            if (e.FinalVelocities.LinearVelocity.X < 0)
            {
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
            Thread.Sleep(1);
            ViewSwitcher.from = ViewSwitcher.Sender.ApptTap;

        }

        private void addBitmapCacheToSDV()
        {
            BitmapCache bmc = new BitmapCache() { RenderAtScale = 0.5 };
            SingleDayWindowBody.CacheMode = bmc;

        }

        private void removeBitmapCacheAfterAnimation()
        {
            Dispatcher.BeginInvoke(delegate
            {
                Thread.Sleep(200);
                SingleDayWindowBody.CacheMode = null;



            });

        }

        private void DayCard_HeaderTap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            ViewSwitcher.from = ViewSwitcher.Sender.HeaderTap;

        }


        private void OpenSdvAndSetTappedDay(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (App.ViewModel.InModus == MainViewModel.Modi.OverView)
                return;
            addBitmapCacheToSDV();
            ViewSwitcher.SwitchToSDV(sender);
            removeBitmapCacheAfterAnimation();

        }
        #endregion

        #region Play Enter / Leave Overview Storyboard Animation

        private List<ItemsControl> foundDayCards_ItemsControll;
        private List<StackPanel> foundStackPanels;



        private void enterOverview()
        {
            App.ViewModel.InModus = MainViewModel.Modi.OverView;
            Storyboard storyboard = AgendaViewBody.Resources["EnterOverview"] as Storyboard;
            storyboard.Begin();

            foundDayCards_ItemsControll = new List<ItemsControl>();
            foundStackPanels = new List<StackPanel>();

            findItemControll(AgendaViewLLS);
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

            findItemControll(AgendaViewLLS);
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

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            toggleOverView();
        }



        private void DefaultAppbar()
        {


            ApplicationBar = new ApplicationBar();
            /*********** MENU ITEMS ***********/
            ApplicationBarMenuItem item1 = new ApplicationBarMenuItem();
            item1.Text = "Einstellungen";
            ApplicationBar.MenuItems.Add(item1);
            ApplicationBarMenuItem item2 = new ApplicationBarMenuItem();
            item2.Text = "Tutorial";
            ApplicationBar.MenuItems.Add(item2);

            ApplicationBarMenuItem item3 = new ApplicationBarMenuItem();
            item3.Text = "Info";
            ApplicationBar.MenuItems.Add(item3);

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

            /*********** SELECT MODE BUTTON ***********/
            ApplicationBarIconButton button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Images/feature.calendar.png", UriKind.Relative);
            button3.Text = "Gehe zu";
            ApplicationBar.Buttons.Add(button3);
            button3.Click += new EventHandler(delegate(object sender, EventArgs e)
            {
                NavigationService.Navigate(new Uri("/MonthView.xaml", UriKind.Relative));
                //App.ViewModel.ClassViewModel.SelectedClass.EnterMultiSelect();
                //App.ViewModel.ClassViewModel.InModus = ClassViewModel.modi.multiSelect;
                //ClassPage_Edit();
            });


            ApplicationBarIconButton button4 = new ApplicationBarIconButton();
            button4.IconUri = new Uri("/Images/feature.search.png", UriKind.Relative);
            button4.Text = "Overview";
            ApplicationBar.Buttons.Add(button4);
            button4.Click += new EventHandler(delegate(object sender, EventArgs e)
            {
                toggleOverView();
            });
        }
        //private void AppointmentsOnGrid_Unloaded(object sender, RoutedEventArgs e)
        //{

        //    ItemsControl itemscontrol = sender as ItemsControl;

        //    itemscontrol.ClearValue(FrameworkElement.DataContextProperty);

        //}





    }
}