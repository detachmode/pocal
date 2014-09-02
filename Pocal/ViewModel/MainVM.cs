using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

using Windows.ApplicationModel.Appointments;
using Pocal.Model;
using System.Windows.Media;
using System.Diagnostics;

namespace Pocal.ViewModel
{

	public class MainVM : INotifyPropertyChanged
	{
		internal int howManyDays = 30;

		internal ObservableCollection<PocalAppointment> PocalAppts = new ObservableCollection<PocalAppointment>();
		public SingleDayViewVM SingleDayViewModel { get; private set; }

		public ObservableCollection<Day> Days { get; private set; }
		public PocalAppointmentManager pocalAppointmentManager = new PocalAppointmentManager();

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

		public MainVM()
		{
			this.Days = new ObservableCollection<Day>();
			SingleDayViewModel = new SingleDayViewVM();


			#region DESIGN TIME DATA
			if (DesignerProperties.IsInDesignTool)
			{
				//	//CREATE DESIGN TIME DATA HERE
				DateTime dt = DateTime.Now - DateTime.Now.TimeOfDay;
				TimeSpan ts = new TimeSpan(1, 30, 0);
				CultureInfo ci = new CultureInfo("de-DE");

				//var appointment = new Windows.ApplicationModel.Appointments.Appointment();
				ObservableCollection<Appointment> DesignDataDayappts = new ObservableCollection<Appointment>();

				SolidColorBrush tempCalColor = new SolidColorBrush(Color.FromArgb(255, 100, 255, 0));

				DesignDataDayappts.Add(new Appointment { Subject = "Geburtstag", StartTime = dt.AddHours(0), AllDay = true });
				DesignDataDayappts.Add(new Appointment { Subject = "Geburtstag", StartTime = dt.AddHours(0), AllDay = true });
				DesignDataDayappts.Add(new Appointment { Subject = "Essen", StartTime = dt.AddHours(9), Duration = ts, Location = "Stuttgart" });
				DesignDataDayappts.Add(new Appointment { Subject = "Einkaufen", StartTime = dt.AddHours(11.3), Duration = ts });
				DesignDataDayappts.Add(new Appointment { Subject = "Mom Anrufen", StartTime = dt.AddHours(14.5), Duration = ts });

				ObservableCollection<PocalAppointment> DesignDataDayPocalappts = new ObservableCollection<PocalAppointment>();
				DesignDataDayPocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[0], CalColor = new SolidColorBrush(Colors.Orange) });
				DesignDataDayPocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[1], CalColor = new SolidColorBrush(Colors.Orange) });
				DesignDataDayPocalappts.Add(new PocalAppointment { Appt = DesignDataDayappts[2], CalColor = tempCalColor });
				//DesignDataDayPocalappts.Add(new PocalAppointment(DesignDataDayappts[2], Color.FromArgb(155, 55, 55, 55)));
				Days.Add(new Day { DT = dt, PocalApptsOfDay = DesignDataDayPocalappts, Sunday = true });

				ObservableCollection<PocalAppointment> DesignDataDayPocalappts1 = new ObservableCollection<PocalAppointment>();
				DesignDataDayPocalappts1.Add(new PocalAppointment { Appt = DesignDataDayappts[4], CalColor =new SolidColorBrush(Colors.Yellow) });
				dt = DateTime.Now.AddDays(1);
				Days.Add(new Day { DT = dt, PocalApptsOfDay = DesignDataDayPocalappts1 });

				ObservableCollection<PocalAppointment> DesignDataDayPocalappts2 = new ObservableCollection<PocalAppointment>();
				dt = dt.AddDays(1);
				Days.Add(new Day { DT = dt, PocalApptsOfDay = DesignDataDayPocalappts2 });
				dt = dt.AddDays(1);
				Days.Add(new Day { DT = dt, PocalApptsOfDay = DesignDataDayPocalappts });

				SingleDayViewModel.TappedDay = Days[1];


				CurrentTop = new Day {DT = dt.AddHours(24) };

			}
			#endregion

		}




		#region PropertyChangedEventHandler / NotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}
}
