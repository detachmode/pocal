﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using Pocal.ViewModel;

namespace Pocal
{
    public partial class SDV_Control : UserControl
    {
        public SDV_Control()
        {
            InitializeComponent();
        }

        public void UpdateScrollviewer()
        {
            this.SingleDayScrollViewer.UpdateLayout();
        }



        private void SingleDayScrollViewer_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (e.FinalVelocities.LinearVelocity.X > 0)
            {
                changeTappedDay(-1);
            }
            if (e.FinalVelocities.LinearVelocity.X < 0)
            {
                changeTappedDay(+1);
            }
        }

        private static void changeTappedDay(int add)
        {
            DateTime nextDate = App.ViewModel.SingleDayViewModel.TappedDay.DT.AddDays(add);
            Day newTappedDay = App.ViewModel.Days.FirstOrDefault(x => x.DT.Date == nextDate.Date);
            if (newTappedDay != null)
                App.ViewModel.SingleDayViewModel.TappedDay = newTappedDay;
            App.ViewModel.ConflictManager.solveConflicts();
        }



    //<!--******** ************ ******** -->
    //<!--******** APPOINTMENTS ******** -->
    //<!--******** ************ ******** -->
    //<ItemsControl x:Name="AppointmentsOnGrid"  ScrollViewer.VerticalScrollBarVisibility="Disabled"   
    //          ItemsSource="{Binding SingleDayViewModel.TappedDay.PocalApptsOfDay, Mode=OneWay}" >
    //    <!--unnötig? NEIN! Sonst fängt die YPos nicht für jedes Element bei Null an. ItemsPanel ist standardmässig ein Stackpanel-->
    //    <ItemsControl.ItemsPanel>
    //        <ItemsPanelTemplate>
    //            <Grid VerticalAlignment="Top">
    //            </Grid>
    //        </ItemsPanelTemplate>
    //    </ItemsControl.ItemsPanel>
    //    <ItemsControl.ItemTemplate>
    //        <DataTemplate>
    //            <pocal:SDV_Appointment_Control>
    //            </pocal:SDV_Appointment_Control>
    //        </DataTemplate>
    //    </ItemsControl.ItemTemplate>
    //</ItemsControl>
        public void RemoveAppointments()
        {
            GridAppointments.Children.Clear();
        }


        public void InsertAppointments()
        {
            if (App.ViewModel.SingleDayViewModel.TappedDay == null)
                return;

            foreach (PocalAppointment pa in App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay)
            {
                SDV_Appointment_Control control = new SDV_Appointment_Control();
                control.DataContext = pa;
                GridAppointments.Children.Add(control);
            }

        }

    }
}
