using System;
using System.Windows.Media;
using Windows.ApplicationModel.Appointments;

namespace Pocal.ViewModel
{
    public class PocalAppointment : ViewModelBase
	{
		private Appointment appt;
		public Appointment Appt
		{
			get
			{
				return appt;
			}
			set
			{
				appt = value;
				this.Subject = appt.Subject;
				this.Location = appt.Location;

				this.StartTime = appt.StartTime;
				this.Duration = appt.Duration;
				NotifyPropertyChanged("Appt");
				
			}
		}

		private string calendarId;

		private SolidColorBrush calColor;
		public SolidColorBrush CalColor
		{
			get
			{
				return calColor;
			}
			set
			{
				calColor = value;
				NotifyPropertyChanged("CalColor");
			}

		}

		private string subject;
		public string Subject
		{
			get
			{
				return subject;
			}
			private set
			{
				if (value != subject)
				{
					subject = value;
					NotifyPropertyChanged("Subject");
				}
			}
		}


		private string location;
		public string Location
		{
			get
			{
				return location;
			}
			private set
			{
				if (value != location)
				{
					location = value;
					NotifyPropertyChanged("Location");
				}
			}
		}

		private DateTimeOffset startTime;
		public DateTimeOffset StartTime
		{
			get
			{
				return startTime;
			}
			private set
			{
				if (value != startTime)
				{
					startTime = value;
					NotifyPropertyChanged("StartTime");
				}
			}
		}


		private TimeSpan duration;
		public TimeSpan Duration
		{
			get
			{
				return duration;
			}
			private set
			{
				if (value != duration)
				{
					duration = value;
					NotifyPropertyChanged("Duration");
				}
			}
		}

		private bool allDay;
		public bool AllDay
		{
			get
			{
				return allDay;
			}
			private set
			{
				if (value != allDay)
				{
					allDay = value;
					NotifyPropertyChanged("AllDay");
				}
			}
		}

	


	
	}
}
