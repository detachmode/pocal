using System.Collections.ObjectModel;
using System.Windows.Controls;
using Pocal.Helper;
using Pocal.ViewModel;
using Shared.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Appointments;

namespace Pocal
{
    public partial class SearchControl : UserControl
    {

        public SearchControl()
        {
            InitializeComponent();
            //LayoutRoot.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;


        }

        public void OpenSearchControl()
        {
            searchBox.Focus();
            if (App.ViewModel.SearchResults == null)
            {
                App.ViewModel.SearchResults = new ObservableCollection<PocalAppointment>();
            }
            SearchAndLoadCache();
        }

        public async void SearchAndLoadCache()
        {
            DateTime startDate = App.ViewModel.LastCachedDate;
            ViewSwitcher.Mainpage.TheSearchControl.SearchLoadingIndicator.Text = "Loading . . .";

            App.ViewModel.CachedAppointmentsForSearch = await CalendarAPI.GetAppointments(startDate, 30);
            DoSearch();

            App.ViewModel.CachedAppointmentsForSearch = await CalendarAPI.GetAppointments(startDate, 365);
            DoSearch();


            ViewSwitcher.Mainpage.TheSearchControl.SearchLoadingIndicator.Text =
                ("Loaded: " + startDate.ToShortDateString() + " - " + startDate.AddDays(365).ToShortDateString());
        }

        public async void SearchAndLoadCachePast()
        {
            DateTime startDate = App.ViewModel.LastCachedDate;
            ViewSwitcher.Mainpage.TheSearchControl.SearchLoadingIndicator.Text = "Loading . . .";

            var list = (await CalendarAPI.GetAppointments(startDate.AddDays(-30), 30)).ToList();
            list.Reverse();
            App.ViewModel.CachedAppointmentsForSearch = list;
            DoSearch();

            list = (await CalendarAPI.GetAppointments(startDate.AddDays(-365), 365)).ToList();
            list.Reverse();
            App.ViewModel.CachedAppointmentsForSearch = list;
            DoSearch();


            ViewSwitcher.Mainpage.TheSearchControl.SearchLoadingIndicator.Text =
                ("Loaded: " + startDate.AddDays(-365).ToShortDateString() + " - " + startDate.ToShortDateString());
        }

        public void CloseSearchControl()
        {
            this.Focus();

        }


        private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            DoSearch();
        }

        private void DoSearch()
        {

            Dispatcher.BeginInvoke(delegate
            {
                App.ViewModel.SearchResults.Clear();
                if (searchBox.Text == "") return;
                PocalAppointmentHelper.SearchCachedAppointments(searchBox.Text);
            });

        }
    }
}
