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

        private static bool openFirstTime = true;

        public static MainPage mainpage = (MainPage)App.RootFrame.Content;
        private static PocalAppointment ScrollToPA;
        public static void setScrollToPa(PocalAppointment pa)
        {
            ScrollToPA = pa;
        }
        private static int offsetFromAllDays;
        private static int additionalOffset = 175;


        private static Day temporaryTappedDay;

        public static void SwitchToSDV(object sender)
        {
            if (currentViewIsSDV())
                return;



            setTemporaryTappedDay(sender);
            Thread.Sleep(1);
            removePreviousDataContext();
            Thread.Sleep(1);

            openSDV();

            scrollToRightPosition();
            if (openFirstTime || temporaryTappedDay.PocalApptsOfDay.Count > 3)
            {

                setTappedDayAsynchron();
                openFirstTime = false;
            }
            else
            {
                Thread.Sleep(1);
                setTappedDay();
            }




        }



        private static void setTemporaryTappedDay(object sender)
        {
            var element = (FrameworkElement)sender;
            temporaryTappedDay = element.DataContext as Day;
        }

        private static void removePreviousDataContext()
        {
            Day blankDay = new Day();
            blankDay.PocalApptsOfDay = null;
            System.Collections.ObjectModel.ObservableCollection<PocalAppointment> tempCollection = new System.Collections.ObjectModel.ObservableCollection<PocalAppointment>();
            foreach (PocalAppointment pa in temporaryTappedDay.PocalApptsOfDay)
            {
                if (pa.AllDay)
                {
                    tempCollection.Add(new PocalAppointment() { AllDay = true });
                }
            }
            blankDay.PocalApptsOfDay = tempCollection;
            App.ViewModel.SingleDayViewModel.TappedDay = blankDay;

        }

        private static bool currentViewIsSDV()
        {
            return mainpage.SingleDayView.Opacity == 100;
        }

        private static void openSDV()
        {
            //Deployment.Current.Dispatcher.BeginInvoke(() =>
            //{
                //Thread.Sleep(100);
                Canvas.SetZIndex(mainpage.SingleDayView, 1);
                //mainpage.SingleDayView.Visibility = Visibility.Visible;
                VisualStateManager.GoToState(mainpage, "OpenDelay", true);
            //});

        }



        private static void setTappedDayAsynchron()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Thread.Sleep(1);
                //Update UI in here as this part will run on the UI thread.
                setTappedDay();

            });

        }

        private static void setTappedDay()
        {
            App.ViewModel.SingleDayViewModel.TappedDay = temporaryTappedDay;
            App.ViewModel.ConflictManager.solveConflicts();
        }



        private static void calculateOffset()
        {
            int allDayCounter = countApptWithAllDay();
            int heightOfOneAppt = 42;

            offsetFromAllDays = heightOfOneAppt * allDayCounter;
        }

        private static int countApptWithAllDay()
        {
            int allDayCounter = 0;
            foreach (var pa in temporaryTappedDay.PocalApptsOfDay)
            {
                if (pa.AllDay == true)
                    allDayCounter += 1;
            }
            return allDayCounter;
        }



        private static void scrollToRightPosition()
        {
            mainpage.SingleDayViewer.UpdateScrollviewer();
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
            mainpage.SingleDayViewer.SingleDayScrollViewer.ScrollToVerticalOffset(ScrollToPA.StartTime.Hour * HourLine.Height - additionalOffset + offsetFromAllDays);
        }

        private static void ScrollTo1200()
        {
            calculateOffset();
            mainpage.SingleDayViewer.SingleDayScrollViewer.ScrollToVerticalOffset(12 * HourLine.Height - additionalOffset + offsetFromAllDays);
        }



    }
}
