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

        private ObservableCollection<DateTime> _months;
        public ObservableCollection<DateTime> Months
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
            Months = new ObservableCollection<DateTime>();
            Months.Add(DateTime.Now);
            Months.Add(DateTime.Now.AddMonths(1));

            //MonthViewPivot.ItemsSource = Months;
        }
    }
}
