﻿using Pocal.Helper;
using Pocal.ViewModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
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


        private static Day temporaryTappedDay;

		public static void SwitchToSDV(object sender)
		{                                                                                  
			if (currentViewIsSDV())
				return;

			openSDV();

            setTemporaryTappedDay(sender);

            removePreviousDataContext();

            scrollToRightPosition();

            setTappedDayAsynchron(sender);

		}

        private static void setTemporaryTappedDay(object sender)
        {
            var element = (FrameworkElement)sender;
            temporaryTappedDay = element.DataContext as Day;
        }

        private static void removePreviousDataContext()
        {
            Day blankDay = new Day();
            blankDay.PocalApptsOfDay = new System.Collections.ObjectModel.ObservableCollection<PocalAppointment>();
            foreach (PocalAppointment pa in temporaryTappedDay.PocalApptsOfDay)
            {
                if (pa.AllDay)
                {
                    blankDay.PocalApptsOfDay.Add(new PocalAppointment() { AllDay = true });
                }
            }
            App.ViewModel.SingleDayViewModel.TappedDay = blankDay;
        }

		private static bool currentViewIsSDV()
		{
			return mainpage.SingleDayView.Opacity == 100;
		}

		private static void openSDV()
		{
			//mainpage.SingleDayView.Visibility = Visibility.Visible;
			VisualStateManager.GoToState(mainpage, "OpenDelay", true);
            
		}

        

		private static void setTappedDayAsynchron(object sender)
		{
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                // Update UI in here as this part will run on the UI thread.
                App.ViewModel.SingleDayViewModel.TappedDay = temporaryTappedDay;
                App.ViewModel.ConflictManager.solveConflicts();
                
            });
            
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
            mainpage.SingleDayScrollViewer.ScrollToVerticalOffset(ScrollToPA.StartTime.Hour * HourLine.Height - additionalOffset + offsetFromAllDays);
		}

		private static void ScrollTo1200()
		{
            calculateOffset();
            mainpage.SingleDayScrollViewer.ScrollToVerticalOffset(12 * HourLine.Height - additionalOffset + offsetFromAllDays);
        }



    }
}
