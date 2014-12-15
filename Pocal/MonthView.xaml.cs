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
            DateTime dt3 = dt.AddMonths(2);

            PivotItem pi = new PivotItem();
            pi.DataContext = dt; 
            MonthViewItem monthViewItem =  new MonthViewItem();
            monthViewItem.loadGridSetup(dt);
            pi.Content = monthViewItem;
            MonthsPivot.Items.Add(pi);


            pi = new PivotItem();
            pi.DataContext = dt2;
            monthViewItem = new MonthViewItem();
            monthViewItem.loadGridSetup(dt2);
            pi.Content = monthViewItem;
            MonthsPivot.Items.Add(pi);

            pi = new PivotItem();
            pi.DataContext = dt3;
            monthViewItem = new MonthViewItem();
            monthViewItem.loadGridSetup(dt3);
            pi.Content = monthViewItem;
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
                backwardPan(pivotItem as PivotItem);

        }

        private void forwardPan(DateTime addedDateTime)
        {
            int index = ((Pivot)MonthsPivot).SelectedIndex;
            int nextIndex;
            if (index == 2)
            {
                nextIndex = 0;
            }
            else
                nextIndex = index + 1;
                   
            DependencyObject nextpivotItem = ((Pivot)MonthsPivot).ItemContainerGenerator.ContainerFromIndex(nextIndex);

            MonthViewItem monthViewItem = new MonthViewItem();
            monthViewItem.loadGridSetup(addedDateTime.AddMonths(1));
            ((PivotItem)nextpivotItem).DataContext = addedDateTime.AddMonths(1);
            ((PivotItem)nextpivotItem).Content = monthViewItem;
        }

        private void backwardPan(PivotItem pivotItem)
        {
            //MonthViewItem monthViewItem = new MonthViewItem();
            //monthViewItem.loadGridSetup(new DateTime(2999, 1, 1));
            //((PivotItem)pivotItem).Content = monthViewItem;
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