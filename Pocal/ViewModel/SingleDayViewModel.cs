﻿using Pocal.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pocal.ViewModel
{
	public class SingleDayViewModel : INotifyPropertyChanged
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


		public SingleDayViewModel()
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
