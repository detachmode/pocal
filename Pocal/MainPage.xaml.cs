using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Pocal.Resources;
using Microsoft.Phone.UserData;
using System.Collections.ObjectModel;
using System.Globalization;
using Pocal.ViewModels;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace Pocal
{
	public partial class MainPage : PhoneApplicationPage
	{
		//List<Day> items;
		private Dictionary<object, ContentPresenter> items;

		//ADDED FROM LISTCALENDAR!!!!!!!!!!!!
		//ADDED FROM LISTCALENDAR!!!!!!!!!!!!
		private List<Appointment> appointemnts = new List<Appointment>();


		//ADDED FROM LISTCALENDAR END !!!!!!!!!!!!
		//ADDED FROM LISTCALENDAR END!!!!!!!!!!!!



		// Constructor
		public MainPage()
		{
			InitializeComponent();


			DataContext = App.ViewModel;
			items = new Dictionary<object, ContentPresenter>();




			App.ViewModel.ShowUpcomingAppointments(30);
			AgendaViewListbox.ItemRealized += LLS_ItemRealized;
			AgendaViewListbox.ItemUnrealized += LLS_ItemUnrealized;


			DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
			dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 40); // TODO performance
			dispatcherTimer.Start();

			//items = App.ViewModel.Days;

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


		public void onDayTap(object sender, System.Windows.Input.GestureEventArgs e)
		{
			//Open SingleDayView
			SingleDayView.Visibility = Visibility.Visible;
			VisualStateManager.GoToState(this, "OpenDelay", true);

			// Get selected Item as Day
			var element = (FrameworkElement)sender;
			Day selectedItem = element.DataContext as Day;

			//Set SingleDayWindowHeader Binding
			App.ViewModel.TappedDay = selectedItem;

			//Set Header Text to Date Format
			//string str = selectedItem.DT.DayOfWeek.ToString();
			//SingleDayWindowHeader.Text = str;
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



	}
}