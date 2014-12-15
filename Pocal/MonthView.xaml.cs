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
      
        public MonthView()
        {
            //DataContext = App.ViewModel.MonthViewModel;

            InitializeComponent();
            addFirstThreePivots();

        }

        private void addFirstThreePivots()
        {

            DateTime dt = DateTime.Now.Date;
            DateTime dt2 = dt.AddMonths(1);
            DateTime dt3 = dt.AddMonths(-1);

            PivotItem pi = new PivotItem();
            pi.DataContext = dt; 
            MonthViewItem monthViewItem =  new MonthViewItem();
            monthViewItem.loadGridSetup(dt);
            pi.Content = monthViewItem;
            pi.Header = dt.ToString("MMMM");
            MonthsPivot.Items.Add(pi);


            pi = new PivotItem();
            pi.DataContext = dt2;
            monthViewItem = new MonthViewItem();
            monthViewItem.loadGridSetup(dt2);
            pi.Content = monthViewItem;
            pi.Header = dt2.ToString("MMMM");
            MonthsPivot.Items.Add(pi);

            pi = new PivotItem();
            pi.DataContext = dt3;
            monthViewItem = new MonthViewItem();
            monthViewItem.loadGridSetup(dt3);
            pi.Content = monthViewItem;
            pi.Header = dt3.ToString("MMMM");
            MonthsPivot.Items.Add(pi);



        }




        
        private static DependencyObject FindMonthGrid(DependencyObject parent)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childCount; i++)
            {
                DependencyObject elt = VisualTreeHelper.GetChild(parent, i);
                if (((FrameworkElement)elt).Name == "MonthViewGrid") return elt;

                var result = FindMonthGrid(elt);
                if (result != null) return result;
            }
            return null;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MonthsPivot.Items

            DependencyObject pivotItem = ((Pivot)sender).ItemContainerGenerator.ContainerFromIndex(((Pivot)sender).SelectedIndex);
            if (pivotItem == null)
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
            PivotItem nextPivotItem = (PivotItem)getNextPivotItem();
            DateTime newDateTime = addedDateTime.AddMonths(1);

            MonthViewItem monthViewItem = new MonthViewItem();
            monthViewItem.loadGridSetup(newDateTime);
            nextPivotItem.DataContext = newDateTime;
            nextPivotItem.Content = monthViewItem;
            nextPivotItem.Header = newDateTime.ToString("MMMM");
        }

        private DependencyObject getNextPivotItem()
        {
            int index = ((Pivot)MonthsPivot).SelectedIndex;
            int nextIndex;
            if (index == 2)
            {
                nextIndex = 0;
            }
            else
                nextIndex = index + 1;

            DependencyObject nextPivotItem = ((Pivot)MonthsPivot).ItemContainerGenerator.ContainerFromIndex(nextIndex);
            return nextPivotItem;
        }



        private void backwardPan(DateTime addedDateTime)
        {

            PivotItem previousPivotItem = (PivotItem)getPreviousPivotItem();
            DateTime newDateTime = addedDateTime.AddMonths(-1);

            MonthViewItem monthViewItem = new MonthViewItem();
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
                previousIndex = 2;
            }
            else
                previousIndex = index - 1;

            DependencyObject previousPivotItem = ((Pivot)MonthsPivot).ItemContainerGenerator.ContainerFromIndex(previousIndex);
            return previousPivotItem;
        }

        //private void loadMonthViewGridInPivotItem(PivotItem pivotItem)
        //{
        //    Month month = pivotItem.DataContext as Month;
        //    var monthViewGrid = FindMonthGrid((DependencyObject)pivotItem);
        //    gridSetup((Grid)monthViewGrid, month.DateTime);
        //}

        //private void Pivot_Loaded(object sender, RoutedEventArgs e)
        //{
        //    DependencyObject pivotItem = ((Pivot)sender).ItemContainerGenerator.ContainerFromIndex(((Pivot)sender).SelectedIndex);

        //    if (pivotItem != null)
        //    {
        //        loadMonthViewGridInPivotItem((PivotItem)pivotItem);
        //    }
        //}


    }
}