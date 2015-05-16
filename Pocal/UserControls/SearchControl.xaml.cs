using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Windows.ApplicationModel.Appointments;
using Pocal.Helper;
using Pocal.ViewModel;

namespace Pocal
{
    public partial class SearchControl : UserControl
    {
        public SearchControl()
        {
            InitializeComponent();
            //LayoutRoot.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            App.ViewModel.SearchNumber = 0;

        }

        public void OpenSearchControl()
        {
            searchBox.Focus();
            App.ViewModel.SearchResults = new ObservableCollection<PocalAppointment>();
        }

        //private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    LayoutRoot.Margin = new Thickness(0, 330, 0, 0);
        //    LayoutRoot.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        //}

        //private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    LayoutRoot.Margin = new Thickness(0, 0, 0, 0);
        //    LayoutRoot.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
        //}
        private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //Appointment appointment = new Appointment {Subject = "Test"};
            //var pocalAppointment = new PocalAppointment
            //{
            //    Appt = appointment
            //};
            
            App.ViewModel.SearchNumber += 1;
            App.ViewModel.SearchResults.Clear();
            if (searchBox.Text == "") return;
            PocalAppointmentHelper.SearchAppointments(searchBox.Text);

        }
    }
}
