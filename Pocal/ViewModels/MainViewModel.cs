using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Collections.ObjectModel;
using System.ComponentModel;
//using Microsoft.Phone.UserData;
using System.Globalization;
using System.Diagnostics;

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Windows.Threading;
using Windows.ApplicationModel.Appointments;


namespace Pocal.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		private int howManyDays = 30;
		private int singleDayViewFirstHour = 8;
		private int singleDayViewLastHour = 20;

		public List<Appointment> appts = new List<Appointment>();

		public ObservableCollection<Day> Days { get; private set; }

		public ObservableCollection<HourListItem> lines { get; private set; }

		//Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

		private Day _tappedDay;
		public Day TappedDay
		{
			get
			{
				return _tappedDay;
			}
			set
			{
				if (value != _tappedDay)
				{
					_tappedDay = value;
					NotifyPropertyChanged("TappedDay");
				}
			}
		}

		private Day _currentTop;
		public Day CurrentTop
		{
			get
			{
				return _currentTop;
			}
			set
			{
				if (value != _currentTop)
				{
					_currentTop = value;
					NotifyPropertyChanged("CurrentTop");
				}
			}
		}

		public MainViewModel()
		{
			this.Days = new ObservableCollection<Day>();
			this.lines = new ObservableCollection<HourListItem>();

			DateTime dt = DateTime.Now - DateTime.Now.TimeOfDay;
			TimeSpan ts = new TimeSpan(2, 0, 0);

			//todo
			TappedDay = new Day { ID = "1", DT = dt.AddHours(300), DayAppts = appts };

			if (DesignerProperties.IsInDesignTool)
			{
				//	//CREATE DESIGN TIME DATA HERE

				CultureInfo ci = new CultureInfo("de-DE");



				var appointment = new Windows.ApplicationModel.Appointments.Appointment();

				appts.Add(new Appointment { Subject = "Geburtstag", StartTime = dt.AddHours(0), AllDay = true });
				appts.Add(new Appointment { Subject = "Geburtstag", StartTime = dt.AddHours(0), AllDay = true });
				appts.Add(new Appointment { Subject = "Essen", StartTime = dt.AddHours(9), Duration = ts, Location = "Stuttgart" });
				appts.Add(new Appointment { Subject = "Einkaufen", StartTime = dt.AddHours(11.3), Duration = ts });
				appts.Add(new Appointment { Subject = "Mom Anrufen", StartTime = dt.AddHours(14.5), Duration = ts });

				Days.Add(new Day { ID = "1", DT = dt, DayAppts = appts, Sunday = true });
				dt = DateTime.Now.AddDays(1);
				Days.Add(new Day { ID = "2", DT = dt, DayAppts = appts });
				dt = dt.AddDays(1);
				Days.Add(new Day { ID = "2", DT = dt, DayAppts = appts });
				dt = dt.AddDays(1);
				Days.Add(new Day { ID = "2", DT = dt, DayAppts = appts });


				CurrentTop = new Day { ID = "3", DT = dt.AddHours(844.5) };

			}

			for (int i = singleDayViewFirstHour; i < singleDayViewLastHour; i++)
			{
				string str = i.ToString("00") + ":00";
				lines.Add(new HourListItem { time = str });
			}

		}

		//public async void ShowCalendars(bool onlyShowVisible)
		//{

		//	IReadOnlyList<AppointmentCalendar> calendars;
		//	if (onlyShowVisible)
		//	{
		//		calendars = await appointmentStore.FindAppointmentCalendarsAsync();
		//	}
		//	else
		//	{
		//		calendars = await appointmentStore.FindAppointmentCalendarsAsync(FindAppointmentCalendarsOptions.IncludeHidden);
		//	}
		//	appts = calendars;
		//}

		public async void ShowUpcomingAppointments(int days)
		{
			var appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);
			FindAppointmentsOptions findOptions = new FindAppointmentsOptions();
			findOptions.MaxCount = 30;
			findOptions.FetchProperties.Add(AppointmentProperties.Subject);
			findOptions.FetchProperties.Add(AppointmentProperties.Location);
			findOptions.FetchProperties.Add(AppointmentProperties.StartTime);
			findOptions.FetchProperties.Add(AppointmentProperties.Duration);

			IReadOnlyList<Appointment> appointments =
				await appointmentStore.FindAppointmentsAsync(DateTime.Now, TimeSpan.FromDays(days), findOptions);

			appts = new List<Appointment>();
			foreach (Appointment a in appointments)
			{
				appts.Add(a);

			}

			fillDayview();
		}


		private void fillDayview()
		{
			DateTime dt = DateTime.Now;
			CultureInfo ci = new CultureInfo("de-DE");

			Days.Clear();
			for (int i = 0; i < howManyDays; i++)
			{
				// Create New Day with its Appointments
				this.Days.Add(new Day()
				{
					ID = i.ToString(),
					DT = dt,
					DayAppts = getApptsOfDay(dt)
				});

				// Sunday Attribute
				if (dt.DayOfWeek == DayOfWeek.Sunday)
				{
					Days[i].Sunday = true;
				}

				// Iteration ++ ein Tag
				dt = dt.AddDays(1);

			}
		}


		private List<Appointment> getApptsOfDay(DateTime dt)
		{


			List<Appointment> thisDayAppts = new List<Appointment>();
			foreach (Appointment a in appts)
			{
				if (a.StartTime.Day == dt.Day)
				{
					thisDayAppts.Add(a);
					//a.StartTime.Minute
				}

			}

			// Sort 
			thisDayAppts = sortAppointments(thisDayAppts);
			this.IsDataLoaded = true;

			return thisDayAppts;
		}

		private List<Appointment> sortAppointments(List<Appointment> appts)
		{
			List<Appointment> sorted = new List<Appointment>();
			IEnumerable<Appointment> query = appts.OrderBy(appt => appt.StartTime);

			foreach (Appointment appt in query)
			{
				sorted.Add(appt);
			}

			return sorted;
		}




		public bool IsDataLoaded
		{
			get;
			private set;
		}


		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
