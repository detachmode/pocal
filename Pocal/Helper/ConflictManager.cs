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
using System.Diagnostics;

namespace Pocal.Helper
{
    public class ConflictManager : INotifyPropertyChanged
    {
        private Dictionary<PocalAppointment, int> Column;
        private Dictionary<PocalAppointment, int> ClusterID;
        private Dictionary<int, int> MaxConflictsPerCluster;
        private int entriesInDictionary;

        private Day day;
        public void solveConflicts()
        {
            Column = new Dictionary<PocalAppointment, int>();
            ClusterID = new Dictionary<PocalAppointment, int>();
            MaxConflictsPerCluster = new Dictionary<int, int>();
            this.day = App.ViewModel.SingleDayViewModel.TappedDay;

            calcColumnsAndClusterID();
            calcMaxConflictsPerCluster();
            calcPropsForBinding();


            toString();

        }

        private void toString()
        {
            Debug.WriteLine("\nColumn:");
            foreach (var item in Column)
            {
                Debug.WriteLine(item.Key.Subject + " " + item.Value);
            }

            Debug.WriteLine("\nClusterID:");
            foreach (var item in ClusterID)
            {
                Debug.WriteLine(item.Key.Subject + " " + item.Value);
            }
            Debug.WriteLine("\nMaxConflictsPerCluster:");
            foreach (var item in MaxConflictsPerCluster)
            {
                Debug.WriteLine(item.Key + " " + item.Value);
            }

        }

        private void calcPropsForBinding()
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

        private void calcColumnsAndClusterID()
        {
            entriesInDictionary = 0;
            foreach (var currentAppt in day.PocalApptsOfDay)
            {
                setValuesOfCurrentAppt(currentAppt);
                entriesInDictionary++;
            }

        }

        private void setValuesOfCurrentAppt( PocalAppointment currentAppt)
        {
            if (entriesInDictionary == 0)
            {
                insertColumn(currentAppt, 1);
                insertClusterID(currentAppt, 1);
            }
            else
            {
                calcColumns(currentAppt);
                calcClusterIDs(currentAppt);

            }
        }

        private void insertClusterID(PocalAppointment currentAppt, int id)
        {
            ClusterID.Add(currentAppt, id);
        }

        private void insertColumn(PocalAppointment currentAppt, int column_n)
        {
            Column.Add(currentAppt, column_n);
        }

        private void calcColumns (PocalAppointment currentAppt)
        {
            for (int column_n = 1; column_n < 5; column_n++)
            {
                if (isCurrentConflictingWithColumn(currentAppt, column_n))
                {
                    if (column_n == 4)
                        insertColumn(currentAppt, 4);
                    continue;
                }
                else
                {
                    insertColumn(currentAppt, column_n);
                    break;
                }
            }
        }

        private bool isCurrentConflictingWithColumn(PocalAppointment currentAppt, int column_n)
        {
            Dictionary<PocalAppointment, int> allInColumn = getFromColumnDicitonaryAllWith(column_n);
            foreach (var entry in allInColumn)
            {
                if (entry.Key != currentAppt)
                {
                    if (isconfliciting(currentAppt, entry.Key))
                        return true;
                }
            }
            return false;

        }



        private void calcClusterIDs(PocalAppointment currentAppt)
        {
            for (int indexInDictionary = entriesInDictionary - 1; indexInDictionary >= 0; indexInDictionary--)
            {

                PocalAppointment previousAppt = day.PocalApptsOfDay[indexInDictionary];

                // Beim letzten Eintrag angekommen
                if (indexInDictionary == 0)
                {
                    if (isconfliciting(currentAppt, previousAppt))
                    {
                        addToClusterAccordingToOtherEntry(currentAppt, previousAppt, 0);
                        break;
                    }
                    else
                    {
                        PocalAppointment lastApptInDictionary = day.PocalApptsOfDay[entriesInDictionary - 1];                     
                        addToClusterAccordingToOtherEntry(currentAppt, lastApptInDictionary, +1);
                        break;
                    }
                }

                if (isconfliciting(currentAppt, previousAppt))
                {          
                    addToClusterAccordingToOtherEntry(currentAppt, previousAppt, 0);
                    break;
                }
            }
        }

        private void addToClusterAccordingToOtherEntry(PocalAppointment currentAppt, PocalAppointment previousAppt, int add)
        {
            int clusterOfPreviousAppt = 0;
            ClusterID.TryGetValue(previousAppt, out clusterOfPreviousAppt);
            ClusterID.Add(currentAppt, clusterOfPreviousAppt + add);

        }

       
        private bool isconfliciting(PocalAppointment currentAppt, PocalAppointment previousAppt)
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
            // ermittle für jeden Eintrag in einem Cluster, was der MaxConflictCount ist
            int maxClusterID = ClusterID.Aggregate((l, r) => l.Value > r.Value ? l : r).Value;
            for (int clusterID = 1; clusterID <= maxClusterID; clusterID++)
            {
                var subsetClusterID = getFromClusterDicitonaryAllWith(clusterID);

                List<int> allColumnsWithSameClusterID = new List<int>();
                foreach (var entry in subsetClusterID)
                {
                    int columnOfAppt;
                    Column.TryGetValue(entry.Key, out columnOfAppt);
                    allColumnsWithSameClusterID.Add(columnOfAppt);

                }
                int maxColumnValue = allColumnsWithSameClusterID.Max();
              
                MaxConflictsPerCluster.Add(clusterID, maxColumnValue);
            }
        }


        private Dictionary<PocalAppointment, int> getFromClusterDicitonaryAllWith(int clusterID)
        {
            Dictionary<PocalAppointment, int> result = ClusterID.Where(pair => pair.Value == clusterID).ToDictionary(r => r.Key, r => r.Value);
            return result;
        }
        private Dictionary<PocalAppointment, int> getFromColumnDicitonaryAllWith(int column_n)
        {
            Dictionary<PocalAppointment, int> result = Column.Where(pair => pair.Value == column_n).ToDictionary(r => r.Key, r => r.Value);
            return result;
        }



        public event PropertyChangedEventHandler PropertyChanged;
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
