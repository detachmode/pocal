using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Pocal.ViewModel;

namespace Pocal.Helper
{
    public static class ViewSwitcher
    {
        public enum Sender
        {
            HeaderTap,
            ApptTap
        }

        public static Sender From;
        public static MainPage Mainpage = (MainPage) App.RootFrame.Content;
        private static PocalAppointment _scrollToPa;
        private static int _offsetFromAllDays;
        private const int AdditionalOffset = 175;

        public static void SetScrollToPa(PocalAppointment pa)
        {
            _scrollToPa = pa;
        }

        public static void SwitchToSdv(object sender)
        {
            if (CurrentViewIsSdv())
                return;

            if (App.ViewModel.InModus == MainViewModel.Modi.OverView)
            {
                App.ViewModel.InModus = MainViewModel.Modi.OverViewSdv;
            }
            else
            {
                App.ViewModel.InModus = MainViewModel.Modi.AgendaViewSdv;
            }

            CalculateOffset();
            Mainpage.SingleDayViewer.PrepareForNewLoadingOfAppoinments();

            OpenSdv();
            SetTappedDay(sender);
            ScrollToRightPosition();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Thread.Sleep(1);
                Mainpage.SingleDayViewer.AddTappedDayAppointments();
            });

            Mainpage.SdvAppbar();
        }

        private static void SetTappedDay(object sender)
        {
            var element = (FrameworkElement) sender;
            App.ViewModel.SingleDayViewModel.TappedDay = element.DataContext as Day;
            App.ViewModel.ConflictManager.SolveConflicts();
        }

        private static bool CurrentViewIsSdv()
        {
            return Mainpage.SingleDayView.Opacity == 100;
        }

        private static void OpenSdv()
        {
            Canvas.SetZIndex(Mainpage.SingleDayView, 1);

            VisualStateManager.GoToState(Mainpage, "OpenDelay", true);
        }

        private static void CalculateOffset()
        {
            var allDayCounter = CountApptWithAllDay();
            var heightOfOneAppt = 48;

            _offsetFromAllDays = heightOfOneAppt*allDayCounter;
        }

        private static int CountApptWithAllDay()
        {
            var allDayCounter = 0;
            foreach (var pa in App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay)
            {
                if (pa.AllDay)
                    allDayCounter += 1;
            }
            return allDayCounter;
        }

        private static void ScrollToRightPosition()
        {
            Mainpage.SingleDayViewer.SDV_ViewportControl.UpdateLayout();
            switch (From)
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
            }
        }

        public static void ScrollToAfterUpdate()
        {
            if (App.ViewModel.SingleDayViewModel.TappedDay == null || _scrollToPa == null)
                return;

            if (App.ViewModel.SingleDayViewModel.TappedDay.Dt.Date == _scrollToPa.StartTime.Date)
                ScrollToApptStartTime();
        }

        private static void ScrollToApptStartTime()
        {
            CalculateOffset();
            SetSdvHeight();
            double y = _scrollToPa.StartTime.Hour*(HourLine.Height + 2) - AdditionalOffset + _offsetFromAllDays;
            Mainpage.SingleDayViewer.SDV_ViewportControl.SetViewportOrigin(new Point(0, y));
            //mainpage.SingleDayViewer.SDV_ScrollViewer.ScrollToVerticalOffset(ScrollToPA.StartTime.Hour * HourLine.Height - additionalOffset + offsetFromAllDays);
        }

        private static void ScrollTo1200()
        {
            CalculateOffset();
            SetSdvHeight();

            double y = 12*(HourLine.Height + 2) - AdditionalOffset + _offsetFromAllDays;
            Mainpage.SingleDayViewer.SDV_ViewportControl.SetViewportOrigin(new Point(0, y));
        }

        private static void SetSdvHeight()
        {
            var screenSizeMultiplicator = App.DisplayInformationEmulator.DisplayInformationEx.ViewPixelsPerHostPixel;
            var width = 480*screenSizeMultiplicator;
            var height = Mainpage.SingleDayViewer.ViewportControlContainer.ActualHeight;
            Mainpage.SingleDayViewer.HourLinesGridAppointments.Width = width;
            Mainpage.SingleDayViewer.SDV_ViewportControl.Bounds = new Rect(0, 0, width, height);
        }
    }
}