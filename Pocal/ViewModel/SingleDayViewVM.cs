using Pocal.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pocal.ViewModel
{
	public class SingleDayViewVM : INotifyPropertyChanged
	{
		internal int FirstHour = 0;
		private int LastHour = 24;

		public ObservableCollection<string> hourLines { get; private set; }

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
			this.hourLines = new ObservableCollection<string>();

			for (int i = FirstHour; i < LastHour; i++)
			{
				string str = i.ToString("00") + ":00";
				hourLines.Add(str);
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
