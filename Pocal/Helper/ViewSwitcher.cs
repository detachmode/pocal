﻿using Pocal.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pocal
{
    public static class ViewSwitcher
    {
        public enum Sender { HeaderTap, ApptTap }
        public static Sender from;

        private static MainPage mainpage = (MainPage)App.RootFrame.Content;
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
            openSDV(mainpage);
            setTappedDay(sender);

            calculateOffset();
            ScrollTo();

            Debug.WriteLine("(OpenSdvAndSetTappedDay)");
        }

        private static bool currentViewIsSDV()
        {
            return mainpage.SingleDayView.Visibility == Visibility.Visible;
        }

        private static void openSDV(MainPage mainpage)
        {
            mainpage.SingleDayView.Visibility = Visibility.Visible;
            VisualStateManager.GoToState(mainpage, "OpenDelay", true);
        }

        private static void setTappedDay(object sender)
        {
            //Set selectedItem 
            var element = (FrameworkElement)sender;
            Day selectedDay = element.DataContext as Day;
            App.ViewModel.SingleDayViewModel.TappedDay = selectedDay;
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
            foreach (var pa in App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay)
            {
                if (pa.AllDay == true)
                    allDayCounter += 1;
            }
            return allDayCounter;
        }



        private static void ScrollTo()
        {
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


        private static void ScrollToApptStartTime()
        {
            mainpage.SingleDayScrollViewer.UpdateLayout();
            mainpage.SingleDayScrollViewer.ScrollToVerticalOffset(ScrollToPA.StartTime.Hour * HourLine.Height - additionalOffset + offsetFromAllDays);
        }

        private static void ScrollTo1200()
        {

            mainpage.SingleDayScrollViewer.ScrollToVerticalOffset(12 * HourLine.Height - additionalOffset + offsetFromAllDays);
        }



    }
}
