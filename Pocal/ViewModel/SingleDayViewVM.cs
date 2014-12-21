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



        public DateTime? getStarTimeFromHourline(string hourLineText)
        {
            string str = hourLineText.Substring(0, 2);
            int hour = -1;

            if (Int32.TryParse(str, out hour))
                if(TappedDay != null)
                    return TappedDay.DT.Date + new TimeSpan(hour, 0, 0);

            //todo
            return null;

        }


    }
}
