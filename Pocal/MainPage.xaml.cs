using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Windows.ApplicationModel.Appointments;
using Pocal.Model;
using Pocal.ViewModel;

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
			watchPositionOfLongListSelector();
			Pocal.ViewModel.AppointmentProvider.ShowUpcomingAppointments();

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


		#endregion

		public async void ApptListItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{

			var element = (FrameworkElement)sender;
			PocalAppointment appt = element.DataContext as PocalAppointment;

			//var store = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);

			//int ind = App.ViewModel.SingleDayViewModel.TappedDay.DayAppts.IndexOf(appt);

			//if (appt.OriginalStartTime == null)
			//{
			//	await store.ShowAppointmentDetailsAsync(appt.LocalId);
			//	App.ViewModel.SingleDayViewModel.TappedDay.DayAppts[ind] = await store.GetAppointmentAsync(appt.LocalId);
			//}
			//else
			//{
			//	await store.ShowAppointmentDetailsAsync(appt.LocalId, appt.OriginalStartTime.Value);
			//	App.ViewModel.SingleDayViewModel.TappedDay.DayAppts[ind] = await store.GetAppointmentInstanceAsync(appt.LocalId, appt.OriginalStartTime.Value);
			//}

			//int i = 1;
			

			 //müll
			//App.ViewModel.TappedDay.DayAppts = App.ViewModel.Days[3].DayAppts;		
			//App.ViewModel.Days[0].DayAppts[0].Subject = "HAllOOO00";
			//App.ViewModel.appts.Remove(appt);
			//App.ViewModel.Days.RemoveAt(1);
			//App.ViewModel.Days[0].DayAppts.Remove(App.ViewModel.appts[0]);

			//App.ViewModel.TappedDay.DayAppts.Remove(appt);

			//OLD Way of updating
			App.ViewModel.SingleDayViewModel.TappedDay = App.ViewModel.Days[1];
			//App.ViewModel.ShowUpcomingAppointments(30);
		
			//foreach (Day d in App.ViewModel.Days)
			//{
			//	if 
			//}

		}




	}
}