using Pocal.Helper;
using Pocal.ViewModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Pocal
{
    public static class ViewSwitcher
    {
        public enum Sender { HeaderTap, ApptTap }
        public static Sender from;

        public static MainPage mainpage = (MainPage)App.RootFrame.Content;
        private static PocalAppointment ScrollToPA;
        public static void setScrollToPa(PocalAppointment pa)
        {
            ScrollToPA = pa;
        }
        private static int offsetFromAllDays;
        private static int additionalOffset = 175;


        public static void SwitchToSDV(object sender)
        {
            if (currentViewIsSDV())
                return;

            if (App.ViewModel.InModus == MainViewModel.Modi.OverView)
            {
                App.ViewModel.InModus = MainViewModel.Modi.OverViewSDV;
            }
            else
            {
                App.ViewModel.InModus = MainViewModel.Modi.AgendaViewSDV;
            }

            calculateOffset();
            mainpage.SingleDayViewer.PrepareForNewLoadingOfAppoinments();
            
            openSDV();
            setTappedDay(sender);
            scrollToRightPosition();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Thread.Sleep(1);
                mainpage.SingleDayViewer.AddTappedDayAppointments();
            });

            mainpage.SDVAppbar();

        }



        private static void setTappedDay(object sender)
        {
            var element = (FrameworkElement)sender;
            App.ViewModel.SingleDayViewModel.TappedDay = element.DataContext as Day;
            App.ViewModel.ConflictManager.solveConflicts();
        }


        private static bool currentViewIsSDV()
        {
            return mainpage.SingleDayView.Opacity == 100;
        }

        private static void openSDV()
        {
            Canvas.SetZIndex(mainpage.SingleDayView, 1);
            
            VisualStateManager.GoToState(mainpage, "OpenDelay", true);


        }



        private static void calculateOffset()
        {
            int allDayCounter = countApptWithAllDay();
            int heightOfOneAppt = 48;

            offsetFromAllDays = heightOfOneAppt * allDayCounter;
        }

        private static int countApptWithAllDay()
        {
            int allDayCounter = 0;
            foreach (var pa in App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay)
            {
                if (pa.AllDay == true)
                    allDayCounter += 1;
            }
            return allDayCounter;
        }



        private static void scrollToRightPosition()
        {
            mainpage.SingleDayViewer.SDV_ScrollViewer.UpdateLayout();
            switch (from)
            {
                case Sender.HeaderTap:
                    {
                        ScrollTo1200();
                        break;
                    }

                case Sender.ApptTap:
                    {
                        ScrollToApptStartTime();
                        break;
                    }

                default:
                    break;
            }

        }

        public static void ScrollToAfterUpdate()
        {
            if (App.ViewModel.SingleDayViewModel.TappedDay == null || ScrollToPA == null)
                return;

            if (App.ViewModel.SingleDayViewModel.TappedDay.DT.Date == ScrollToPA.StartTime.Date)
                ScrollToApptStartTime();
        }


        private static void ScrollToApptStartTime()
        {
            calculateOffset();
            setSdvHeight();
            double y = ScrollToPA.StartTime.Hour * (HourLine.Height + 2) - additionalOffset + offsetFromAllDays;
            mainpage.SingleDayViewer.SDV_ScrollViewer.SetViewportOrigin(new Point(0,y));
            //mainpage.SingleDayViewer.SDV_ScrollViewer.ScrollToVerticalOffset(ScrollToPA.StartTime.Hour * HourLine.Height - additionalOffset + offsetFromAllDays);
        }

        private static void ScrollTo1200()
        {
            calculateOffset();
            setSdvHeight();

            double y = 12 * (HourLine.Height + 2) - additionalOffset + offsetFromAllDays; 
            mainpage.SingleDayViewer.SDV_ScrollViewer.SetViewportOrigin(new Point(0, y));
        }

        private static void setSdvHeight()
        {
            double height = 24 * (HourLine.Height + 2) + offsetFromAllDays +4;
            mainpage.SingleDayViewer.SDV_ScrollViewer.Bounds = new Rect(0, 0, 500, height);
        }





    }
}
