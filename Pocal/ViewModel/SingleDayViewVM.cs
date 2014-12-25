using System;
using System.Collections.ObjectModel;

namespace Pocal.ViewModel
{
    public class SingleDayViewVM : ViewModelBase
    {

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



        public DateTime? getStarTimeFromHourline(HourLine hourLine)
        {

            return TappedDay.DT.Date.AddHours(hourLine.DT.Hour);

        }


    }
}
