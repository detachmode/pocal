using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using Microsoft.Xna.Framework;
using Pocal.Converter;
using System.Collections.ObjectModel;
using Pocal.ViewModel;

namespace Pocal
{
    public partial class MonthView : PhoneApplicationPage
    {
        public static  MonthView CurrentPage;
        PivotItem pivotItem;
        MonthViewUserControl monthViewUserControl;

        public MonthView()
        {

            InitializeComponent();
            CurrentPage = this;
            addFirstThreePivots();

        }



        private void addFirstThreePivots()
        {

            DateTime dt = DateTime.Now.Date;
            DateTime dt2 = dt.AddMonths(1);
            DateTime dt3 = dt.AddMonths(2);
            DateTime dt4 = dt.AddMonths(3);
            DateTime dt5 = dt.AddMonths(-1);


            addPivotItem(dt);
            addPivotItem(dt2);
            addPivotItem(dt3);
            addPivotItem(dt4);
            addPivotItem(dt5);


        }

        private void addPivotItem(DateTime dt)
        {
            monthViewUserControl = new MonthViewUserControl();
            monthViewUserControl.loadGridSetup(dt);
            
            pivotItem = new PivotItem();
            pivotItem.Content = monthViewUserControl;
            pivotItem.DataContext = dt;
            pivotItem.Margin = new Thickness(0, 0, 0, 0);
            pivotItem.Header = dt.ToString("MMMM");

            MonthsPivot.Items.Add(pivotItem);
        }


        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DependencyObject selectedPivotItem = ((Pivot)sender).ItemContainerGenerator.ContainerFromIndex(((Pivot)sender).SelectedIndex);
            if (selectedPivotItem == null)
                return;


            DateTime removedDateTime =  (DateTime)(e.RemovedItems[0] as PivotItem).DataContext;
            DateTime addedDateTime =  (DateTime)(e.AddedItems[0] as PivotItem).DataContext;
            if (removedDateTime < addedDateTime)
            {
                forwardPan(addedDateTime);
            }
            else
                backwardPan(addedDateTime);

        }

        private void forwardPan(DateTime addedDateTime)
        {
            PivotItem lastPivotItem = (PivotItem)getLastPivotItem();
            DateTime newDateTime = addedDateTime.AddMonths(3);

            MonthViewUserControl monthViewItem = new MonthViewUserControl();
            monthViewItem.loadGridSetup(newDateTime);
            lastPivotItem.DataContext = newDateTime;
            lastPivotItem.Content = monthViewItem;
            lastPivotItem.Header = newDateTime.ToString("MMMM");
        }

        private DependencyObject getLastPivotItem()
        {
            int index = ((Pivot)MonthsPivot).SelectedIndex;
            int lastIndex;

            lastIndex = (index + 3)%5;

            DependencyObject lastPivotItem = ((Pivot)MonthsPivot).ItemContainerGenerator.ContainerFromIndex(lastIndex);
            return lastPivotItem;
        }



        private void backwardPan(DateTime addedDateTime)
        {

            PivotItem previousPivotItem = (PivotItem)getPreviousPivotItem();
            DateTime newDateTime = addedDateTime.AddMonths(-1);

            MonthViewUserControl monthViewItem = new MonthViewUserControl();
            monthViewItem.loadGridSetup(newDateTime);
            previousPivotItem.DataContext = newDateTime;
            previousPivotItem.Content = monthViewItem;
            previousPivotItem.Header = newDateTime.ToString("MMMM");
        }

        private DependencyObject getPreviousPivotItem()
        {
            int index = ((Pivot)MonthsPivot).SelectedIndex;
            int previousIndex;
            if (index == 0)
            {
                previousIndex = 4;
            }
            else
                previousIndex = index - 1;

            DependencyObject previousPivotItem = ((Pivot)MonthsPivot).ItemContainerGenerator.ContainerFromIndex(previousIndex);
            return previousPivotItem;
        }


        public void navigateBackToDay(DateTime dt)
        {
            App.ViewModel.GoToDate(dt);
            NavigationService.Navigate(new Uri("/Mainpage.xaml?GoToDate=", UriKind.Relative), dt);
        }


    }
}