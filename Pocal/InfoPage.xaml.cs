using System;
using System.Windows;
using Microsoft.Phone.Tasks;

namespace Pocal
{
    public partial class InfoPage
    {
        public InfoPage()
        {
            InitializeComponent();
        }

        private void Email_OnClick(object sender, RoutedEventArgs e)
        {
            var emailComposeTask = new EmailComposeTask
            {
                Subject = "Pocal Feedback",
                Body = "",
                To = "dennis.briefkasten@gmail.com"
            };
            emailComposeTask.Show();
        }

        private void Store_OnClick(object sender, RoutedEventArgs e)
        {
            App._marketPlaceDetailTask.Show();
        }

        private void Twitter_OnClick(object sender, RoutedEventArgs e)
        {
            var webBrowserTask = new WebBrowserTask {Uri = new Uri("https://twitter.com/Detachmode", UriKind.Absolute)};

            webBrowserTask.Show();
        }
    }
}