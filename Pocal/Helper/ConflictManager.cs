using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Pocal.ViewModel;

namespace Pocal.Helper
{
    public class ConflictManager : INotifyPropertyChanged
    {
        private Dictionary<PocalAppointment, int> _clusterId;
        private Dictionary<PocalAppointment, int> _column;
        private Day _day;
        private int _entriesInDictionary;
        private Dictionary<int, int> _maxConflictsPerCluster;
        public event PropertyChangedEventHandler PropertyChanged;

        public void SolveConflicts()
        {
            _day = App.ViewModel.SingleDayViewModel.TappedDay;
            if (_day == null)
                return;

            if (_day.PocalApptsOfDay.Count == 0)
                return;

            _column = new Dictionary<PocalAppointment, int>();
            _clusterId = new Dictionary<PocalAppointment, int>();
            _maxConflictsPerCluster = new Dictionary<int, int>();


            CalcColumnsAndClusterId();
            CalcMaxConflictsPerCluster();
            CalcPropsForBinding();


            //toString();
        }

/*
        private void toString()
        {
            Debug.WriteLine("\nColumn:");
            foreach (var item in _column)
            {
                Debug.WriteLine(item.Key.Subject + " " + item.Value);
            }

            Debug.WriteLine("\nClusterID:");
            foreach (var item in _clusterId)
            {
                Debug.WriteLine(item.Key.Subject + " " + item.Value);
            }
            Debug.WriteLine("\nMaxConflictsPerCluster:");
            foreach (var item in _maxConflictsPerCluster)
            {
                Debug.WriteLine(item.Key + " " + item.Value);
            }

        }
*/

        private void CalcPropsForBinding()
        {
            foreach (var pa in _day.PocalApptsOfDay)
            {
                var clusterid = _clusterId.First(x => x.Key == pa).Value;
                var foundMaxConflicts = _maxConflictsPerCluster.First(x => x.Key == clusterid).Value;
                pa.MaxConflicts = foundMaxConflicts;

                var column = _column.First(x => x.Key == pa).Value;
                pa.Column = column;
            }
        }

        private void CalcColumnsAndClusterId()
        {
            _entriesInDictionary = 0;
            foreach (var currentAppt in _day.PocalApptsOfDay)
            {
                if (_entriesInDictionary == 0)
                {
                    InsertColumn(currentAppt, 1);
                    InsertClusterId(currentAppt, 1);
                }
                else
                {
                    CalcColumns(currentAppt);
                    CalcClusterIDs(currentAppt);
                }
                _entriesInDictionary++;
            }
        }

        private void InsertClusterId(PocalAppointment currentAppt, int id)
        {
            _clusterId.Add(currentAppt, id);
        }

        private void InsertColumn(PocalAppointment currentAppt, int columnN)
        {
            _column.Add(currentAppt, columnN);
        }

        private void CalcColumns(PocalAppointment currentAppt)
        {
            for (var columnN = 1; columnN < 5; columnN++)
            {
                if (IsCurrentConflictingWithColumn(currentAppt, columnN))
                {
                    if (columnN == 4)
                        InsertColumn(currentAppt, 4);
                }
                else
                {
                    InsertColumn(currentAppt, columnN);
                    break;
                }
            }
        }

        private bool IsCurrentConflictingWithColumn(PocalAppointment currentAppt, int columnN)
        {
            var allInColumn = GetFromColumnDicitonaryAllWith(columnN);
            return
                allInColumn.Where(entry => entry.Key != currentAppt)
                    .Any(entry => Isconfliciting(currentAppt, entry.Key));
        }

        private void CalcClusterIDs(PocalAppointment currentPocalAppointment)
        {
            for (var indexInDictionary = _entriesInDictionary - 1; indexInDictionary >= 0; indexInDictionary--)
            {
                var pocalAppointment = _day.PocalApptsOfDay[indexInDictionary];

                // Beim letzten Eintrag angekommen
                if (indexInDictionary == 0)
                {
                    if (Isconfliciting(currentPocalAppointment, pocalAppointment))
                    {
                        AddToClusterAccordingToOtherEntry(currentPocalAppointment, pocalAppointment, 0);
                        break;
                    }
                    var lastApptInDictionary = _day.PocalApptsOfDay[_entriesInDictionary - 1];
                    AddToClusterAccordingToOtherEntry(currentPocalAppointment, lastApptInDictionary, +1);
                    break;
                }

                if (!Isconfliciting(currentPocalAppointment, pocalAppointment)) continue;

                AddToClusterAccordingToOtherEntry(currentPocalAppointment, pocalAppointment, 0);
                break;
            }
        }

        private void AddToClusterAccordingToOtherEntry(PocalAppointment currentAppt, PocalAppointment previousAppt,
            int add)
        {
            int clusterOfPreviousAppt;
            _clusterId.TryGetValue(previousAppt, out clusterOfPreviousAppt);
            _clusterId.Add(currentAppt, clusterOfPreviousAppt + add);
        }

        private static bool Isconfliciting(PocalAppointment currentAppt, PocalAppointment previousAppt)
        {
            var currentStart = currentAppt.StartTime.DateTime;
            var currentEnd = currentStart + currentAppt.Duration;

            // Wenn zwei PAs sich überschneiden, 
            return currentAppt != previousAppt && previousAppt.isInTimeFrame_IgnoreAllDays(currentStart, currentEnd);
        }

        private void CalcMaxConflictsPerCluster()
        {
            // ermittle für jeden Eintrag in einem Cluster, was der MaxConflictCount ist
            var maxClusterId = _clusterId.Aggregate((l, r) => l.Value > r.Value ? l : r).Value;
            for (var clusterId = 1; clusterId <= maxClusterId; clusterId++)
            {
                var subsetClusterId = GetFromClusterDicitonaryAllWith(clusterId);

                var allColumnsWithSameClusterId = new List<int>();
                foreach (var entry in subsetClusterId)
                {
                    int columnOfAppt;
                    _column.TryGetValue(entry.Key, out columnOfAppt);
                    allColumnsWithSameClusterId.Add(columnOfAppt);
                }
                var maxColumnValue = allColumnsWithSameClusterId.Max();

                _maxConflictsPerCluster.Add(clusterId, maxColumnValue);
            }
        }

        private Dictionary<PocalAppointment, int> GetFromClusterDicitonaryAllWith(int clusterId)
        {
            var result = _clusterId.Where(pair => pair.Value == clusterId).ToDictionary(r => r.Key, r => r.Value);
            return result;
        }

        private Dictionary<PocalAppointment, int> GetFromColumnDicitonaryAllWith(int columnN)
        {
            var result = _column.Where(pair => pair.Value == columnN).ToDictionary(r => r.Key, r => r.Value);
            return result;
        }

        internal void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}