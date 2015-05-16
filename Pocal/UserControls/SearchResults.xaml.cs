﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Pocal.Helper;
using Pocal.ViewModel;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace Pocal
{
    public partial class SearchResults : UserControl
    {
        public SearchResults()
        {
            InitializeComponent();
        }

        private void LayoutRoot_OnTap(object sender, GestureEventArgs e)
        {
            var pocalAppointment = (PocalAppointment) LayoutRoot.DataContext;
            ViewSwitcher.Mainpage.CloseSearchView();
            App.ViewModel.GoToDate(pocalAppointment.StartTime.DateTime);
        }
    }
}
