using System;
using Pocal.Helper;

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
                if (value == _tappedDay) return;
                _tappedDay = value;
                NotifyPropertyChanged("TappedDay");
            }
        }



        public DateTime? GetStarTimeFromHourline(HourLine hourLine)
        {

            return TappedDay.Dt.Date.AddHours(hourLine.Dt.Hour);

        }


    }
}
