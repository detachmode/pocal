using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.ApplicationModel.Appointments;

namespace Pocal.ViewModel
{
	public class PocalAppointment : INotifyPropertyChanged
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
				
			}
		}

		private string calendarId;

		private Color calColor;
		public Color CalColor
		{
			get
			{
				return calColor;
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



		public PocalAppointment(Appointment appt , Color calColor)
		{
			this.appt = appt;
			this.calColor = calColor;
			this.calendarId = appt.CalendarId;

			this.subject = appt.Subject;
			this.location = appt.Location;

			this.startTime = appt.StartTime;
			this.duration = appt.Duration;
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
