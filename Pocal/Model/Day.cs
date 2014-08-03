//using Microsoft.Phone.UserData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.ApplicationModel.Appointments;

namespace Pocal.Model
{
    public class Day : INotifyPropertyChanged
    {
        
        private string _id;
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

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

		private ObservableCollection<Appointment> _dayAppts;
		public ObservableCollection<Appointment> DayAppts
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
