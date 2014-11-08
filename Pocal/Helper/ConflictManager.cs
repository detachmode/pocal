using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pocal.ViewModel;
using Windows.ApplicationModel.Appointments;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows;

namespace Pocal.Helper
{
    public class ConflictManager : INotifyPropertyChanged
	{
		private  Dictionary<PocalAppointment, int> Column;
		private  Dictionary<PocalAppointment, int> ClusterID;
		private  Dictionary<int, int> MaxConflictsPerCluster;

         private Day day;
		public  void solveConflicts()
		{
			Column = new Dictionary<PocalAppointment, int>();
			ClusterID = new Dictionary<PocalAppointment, int>();
			MaxConflictsPerCluster = new Dictionary<int, int>();
            this.day = App.ViewModel.SingleDayViewModel.TappedDay; 

			calcColumnsAndClusterID();
			calcMaxConflictsPerCluster();
            calcPropsForBinding();


		}

		private  void calcPropsForBinding()
		{
            foreach (PocalAppointment pa in day.PocalApptsOfDay)
            {
                int clusterid = ClusterID.First(x => x.Key == pa).Value;
                int foundMaxConflicts = MaxConflictsPerCluster.First(x => x.Key == clusterid).Value;
                pa.MaxConflicts = foundMaxConflicts;

                int column = Column.First(x => x.Key == pa).Value;
                pa.Column = column;
            }
		}

		private  void testData()
		{
			DateTime start = new DateTime(2014, 11, 07);
			DateTime dt = start - start.TimeOfDay;
			DateTime dt0 = dt;
			TimeSpan ts = new TimeSpan(1, 30, 0);
			TimeSpan ts2 = new TimeSpan(2, 0, 0);
			TimeSpan ts3 = new TimeSpan(3, 0, 0);


            //allAppointments.Add(new PocalAppointment { Appt = new Appointment { Subject = "Gameplay Programming", StartTime = dt0.AddHours(8.5), Duration = ts3 } });
            //allAppointments.Add(new PocalAppointment { Appt = new Appointment { Subject = "Gameplay Programming", StartTime = dt0.AddHours(9.5), Duration = ts3 } });
            //allAppointments.Add(new PocalAppointment { Appt = new Appointment { Subject = "Gameplay Programming", StartTime = dt0.AddHours(10.5), Duration = ts3 } });
            //allAppointments.Add(new PocalAppointment { Appt = new Appointment { Subject = "Gameplay Programming", StartTime = dt0.AddHours(18.5), Duration = ts3 } });




		}
		private  void calcColumnsAndClusterID()
		{
			int i = 0;
            foreach (var currentAppt in day.PocalApptsOfDay)
			{
				setValuesOfCurrentAppt(i, currentAppt);
				i++;
			}

		}

		private  void setValuesOfCurrentAppt(int i, PocalAppointment currentAppt)
		{
			if (i == 0)
			{
				Column.Add(currentAppt, 1);
				ClusterID.Add(currentAppt, 1);
			}
			else
				iterateOverPreviouseEntries(i, currentAppt);
		}

		private  void iterateOverPreviouseEntries(int i, PocalAppointment currentAppt)
		{
			for (int j = i - 1; j >= 0; j--)
			{
                PocalAppointment previousAppt = day.PocalApptsOfDay[j];
				if (isconfliciting(currentAppt, previousAppt))
				{
					addToColumnAccordingToOtherEntry(currentAppt, previousAppt, 1);
					addToClusterAccordingToOtherEntry(currentAppt, previousAppt, 0);
					break;

				}
				if (j == 0)
				{
					Column.Add(currentAppt, 1);
					addToClusterAccordingToOtherEntry(currentAppt, previousAppt, 1);

				}
			}
		}

		private  void addToColumnAccordingToOtherEntry(PocalAppointment currentAppt, PocalAppointment previousAppt, int add)
		{
			int columnOfPreviousAppt = 0;
			Column.TryGetValue(previousAppt, out columnOfPreviousAppt);
			Column.Add(currentAppt, add + columnOfPreviousAppt);
		}

		private  void addToClusterAccordingToOtherEntry(PocalAppointment currentAppt, PocalAppointment previousAppt, int add)
		{
			int clusterOfPreviousAppt = 0;
			ClusterID.TryGetValue(previousAppt, out clusterOfPreviousAppt);
			ClusterID.Add(currentAppt, clusterOfPreviousAppt + add);

		}

		private  bool isconfliciting(PocalAppointment currentAppt, PocalAppointment previousAppt)
		{

			DateTime currentStart = currentAppt.StartTime.DateTime;
			DateTime currentEnd = currentStart + currentAppt.Duration;

			// Wenn zwei PAs sich überschneiden, 
			if (currentAppt != previousAppt && previousAppt.isInTimeFrame(currentStart, currentEnd))
				return true;

			return false;

		}




		private void calcMaxConflictsPerCluster()
		{
            for (int clusterID = 1; clusterID <= day.PocalApptsOfDay.Count; clusterID++)
			{
				int clusterCount = ClusterID.Count(x => x.Value == clusterID);
				if (clusterCount != 0)
				{
					MaxConflictsPerCluster.Add(clusterID, clusterCount);
				}

			}
		}


        public  event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
	}
}
