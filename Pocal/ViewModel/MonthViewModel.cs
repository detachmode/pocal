using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pocal.ViewModel
{
    public class MonthViewModel
    {
        public ObservableCollection<DateTime> Months { get; set; }

        public MonthViewModel()
        {
            Months = new ObservableCollection<DateTime>();
            Months.Add(DateTime.Now);
            Months.Add(DateTime.Now.AddMonths(1));

            //MonthViewPivot.ItemsSource = Months;
        }
    }
}
