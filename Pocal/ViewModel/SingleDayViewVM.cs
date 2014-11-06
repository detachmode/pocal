using System;
using System.Collections.ObjectModel;

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

                triggerScrollToOffset(EventArgs.Empty);
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
                hourLines.Add(new HourLine { Text = str });
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



        public event EventHandler TriggerScrollToOffset;
        protected virtual void triggerScrollToOffset(EventArgs args)
        {
            var handler = this.TriggerScrollToOffset;
            if (handler != null)
            {
                handler(this, args);
            }
        }



    }
}
