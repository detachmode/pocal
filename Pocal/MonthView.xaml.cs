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

namespace Pocal
{
    public partial class MonthView : PhoneApplicationPage
    {
        public MonthView()
        {
            InitializeComponent();
            gridSetup();
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml?GoToDate=" + (DateTime.Now.Date.ToShortDateString()), UriKind.Relative));


        }

        private void gridSetup()
        {
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    Border brd = new Border();


                    int leftThinkness = 2;
                    int topThinkness = 2;
                    int rightThinkness = 0;
                    int bottomThinkness = 2;

                    if (y != 4)
                        bottomThinkness = 0;

                    if (x == 0)
                        leftThinkness = 0;



                    brd.BorderThickness = new Thickness(leftThinkness, topThinkness, rightThinkness, bottomThinkness);
                    brd.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    brd.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    brd.BorderBrush = new SolidColorBrush(Colors.White);
                    double height = (483 / 7);
                    double width = (483 / 7);
                    brd.Height = height;
                    brd.Width = width;
                    double leftmargin = height * x;
                    double topmargin = width * y;
                    brd.Margin = new Thickness(leftmargin, topmargin, 0, 0);
                    (MonthViewGrid as Grid).Children.Add(brd);
                }
            }
        }
    }
}