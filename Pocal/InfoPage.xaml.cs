﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace Pocal
{
    public partial class InfoPage : PhoneApplicationPage
    {
        public InfoPage()
        {
            InitializeComponent();
        }

        private void Email_OnClick(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.Subject = "Pocal Feedback";
            emailComposeTask.Body = "";
            emailComposeTask.To = "dennis.briefkasten@gmail.com";
            emailComposeTask.Show();
        }

        private void Store_OnClick(object sender, RoutedEventArgs e)
        {
            App._marketPlaceDetailTask.Show();
        }

    }
}