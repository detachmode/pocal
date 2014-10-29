using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pocal.ViewModel
{
    public class SingleDayViewVM : ViewModelBase
    {
        internal int FirstHour = 0;
        private int LastHour = 24;

        public ObservableCollection<HourLine> hourLines { get; private set; }

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


        public SingleDayViewVM()
        {
            gridSetup();
        }


        internal void gridSetup()
        {
            this.hourLines = new ObservableCollection<HourLine>();

            for (int i = FirstHour; i < LastHour; i++)
            {
                string str = i.ToString("00") + ":00";
                hourLines.Add(new HourLine { Text = str, Height = 70 });
            }

        }


        public DateTime getStarTimeFromHourline(string hourLineText)
        {
            string str = hourLineText.Substring(0, 2);
            int hour = -1;

            if (Int32.TryParse(str, out hour))
                return TappedDay.DT.Date + new TimeSpan(hour, 0, 0);

            //todo
            return DateTime.Now;

        }

        public void updateTappedDay()
        {

            if (TappedDay == null)
            {
                TappedDay = App.ViewModel.Days[0];
            }
            else
            {
                DateTime dt = TappedDay.DT;
                Day td = App.ViewModel.Days.FirstOrDefault(x => x.DT.Date == dt.Date);
                TappedDay = td;
            }
        }


    }

    public class HourLine
    {
        public string Text { get; set; }
        public int Height { get; set; }

    }
}
