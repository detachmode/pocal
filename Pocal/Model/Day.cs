using Pocal.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using Windows.ApplicationModel.Appointments;

namespace Pocal.Model
{

    public class Day : INotifyPropertyChanged
    {

		private bool _isHighlighted;
        public bool IsHighlighted
        {
            get
            {
                return _isHighlighted;
            }
            set
            {
                if (value != _isHighlighted)
                {
                    _isHighlighted = value;
                    NotifyPropertyChanged("IsHighlighted");
                }
            }
        }

        public bool Sunday = false;

        public object customBackground
        {
            get
            {
                if (Sunday )//RED 
                {
                    return new SolidColorBrush(Color.FromArgb(255, 122, 122, 122));
                }

                // Color por defecto
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object sundayForeground
        {
            get
            {
                if (Sunday)//RED 
                {
                    return new SolidColorBrush(Color.FromArgb(255, 255, 3, 3));
                }

                // Color por defecto
                return new SolidColorBrush(Colors.White);
            }
        }

        private DateTime _dt;
        public DateTime DT
        {
            get
            {
                return _dt;
            }
            set
            {
                if (value != _dt)
                {
                    _dt = value;
                    NotifyPropertyChanged("DT");
                }
            }
        }

		private ObservableCollection<PocalAppointment> _dayAppts;
		public ObservableCollection<PocalAppointment> DayAppts
        {
            get
            {
                return _dayAppts;
            }

            set
            {
                if (value != _dayAppts)
                {
                    _dayAppts = value;
                    
                    NotifyPropertyChanged("DayAppts");
                }

            }
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
