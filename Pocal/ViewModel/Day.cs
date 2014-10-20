using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Pocal.ViewModel
{

    public class Day : ViewModelBase
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

        public object sundayForeground
        {
            get
            {
                if (Sunday)//RED 
                {


					return (SolidColorBrush)App.Current.Resources["PhoneAccentBrush"];


					//return new SolidColorBrush(Color.FromArgb(255, 255, 3, 3));
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
		public ObservableCollection<PocalAppointment> PocalApptsOfDay
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
                    
                    NotifyPropertyChanged("PocalApptsOfDay");
                }

            }
        }


	}
}
