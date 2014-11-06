using System;
using System.Collections.ObjectModel;

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
