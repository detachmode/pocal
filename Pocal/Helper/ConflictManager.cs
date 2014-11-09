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
            int entriesInDictionary = 0;
            foreach (var currentAppt in day.PocalApptsOfDay)
            {
                setValuesOfCurrentAppt(entriesInDictionary, currentAppt);
                entriesInDictionary++;
            }

        }

        private void setValuesOfCurrentAppt(int entriesInDictionary, PocalAppointment currentAppt)
        {
            if (entriesInDictionary == 0)
            {
                insertColumn(currentAppt, 1);
                insertClusterID(currentAppt, 1);
            }
            else
                iterateOverPreviouseEntries(entriesInDictionary, currentAppt);
        }

        private void insertClusterID(PocalAppointment currentAppt, int value)
        {
            ClusterID.Add(currentAppt, value);
        }

        private void insertColumn(PocalAppointment currentAppt, int value)
        {
            Column.Add(currentAppt, value);
        }

        private void iterateOverPreviouseEntries(int entriesInDictionary, PocalAppointment currentAppt)
        {
            for (int indexInDictionary = entriesInDictionary - 1; indexInDictionary >= 0; indexInDictionary--)
            {

                // Falls er mit vorherigen Eintrag nicht conflictet, wiederhole solange, bis er ein Eintrag gefunden hat, oder bei 0 angekommen ist.
                // Wenn er bei 0 angekommen und Termin ist konfliktfrei dann kann er mit neuer ClusterID und Column 1 eingetragen werden.
                // Wenn er bei 0 angekommen und Termin ist NICHT konfliktfrei dann kann er mit gleicher ClusterID wie Konflikt und Column+1 eingetragen werden.

                PocalAppointment previousAppt = day.PocalApptsOfDay[indexInDictionary];

                // Beim letzten Eintrag angekommen
                if (indexInDictionary == 0)
                {
                    if (isconfliciting(currentAppt, previousAppt))
                    {
                        addToColumnAccordingToOtherEntry(currentAppt, previousAppt, +1);
                        addToClusterAccordingToOtherEntry(currentAppt, previousAppt, 0);
                        break;
                    }
                    else
                    {
                        PocalAppointment lastApptInDictionary = day.PocalApptsOfDay[entriesInDictionary - 1];
                        insertColumn(currentAppt, 1);
                        addToClusterAccordingToOtherEntry(currentAppt, lastApptInDictionary, +1);
                        break;
                    }
                }

                if (isconfliciting(currentAppt, previousAppt))
                {
                    addToColumnAccordingToOtherEntry(currentAppt, previousAppt, +1);
                    addToClusterAccordingToOtherEntry(currentAppt, previousAppt, 0);
                    break;
                }
            }
        }

        private void addToColumnAccordingToOtherEntry(PocalAppointment currentAppt, PocalAppointment previousAppt, int add)
        {
            int columnOfPreviousAppt = 0;
            Column.TryGetValue(previousAppt, out columnOfPreviousAppt);
            Column.Add(currentAppt, add + columnOfPreviousAppt);
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
            for (int clusterID = 1; clusterID <= day.PocalApptsOfDay.Count; clusterID++)
            {
                int clusterCount = ClusterID.Count(x => x.Value == clusterID);
                if (clusterCount != 0)
                {
                    MaxConflictsPerCluster.Add(clusterID, clusterCount);
                }

            }
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
