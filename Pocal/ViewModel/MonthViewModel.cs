using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pocal.ViewModel
{
    public class MonthViewModel : ViewModelBase
    {

        public static int Counter = 0;

        private ObservableCollection<Month> _months;
        public ObservableCollection<Month> Months
        {
            get
            {
                return _months;
            }

            set
            {
                if (_months != value)
                {
                    _months = value;
                    NotifyPropertyChanged("Months");
                }
                    

            }
        }

       

        public MonthViewModel()
        {
            Months = new ObservableCollection<Month>();

            for (int i = 0; i < 6; i++)
            {
                Months.Add(new Month(DateTime.Now.AddMonths(i))); 
            }
            Months.Add(new Month(DateTime.Now.AddMonths(-1))); 

            //MonthViewPivot.ItemsSource = Months;
        }
    }

    public class Month
    {
        public Month(DateTime dt)
        {
            DateTime = dt;
            Name = dt.ToString("MMMMMM"); ;
        }

        public string Name { get; set; }
        public DateTime DateTime { get; set; }
    }
}
