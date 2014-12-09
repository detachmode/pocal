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

        private List<int> daysForGridSetup;
        private List<DateTime> daysForGridSetupDateTimes;

        private void gridSetup()
        {
            daysForGridSetup = createDaysArray(new DateTime(2014,12,1));
            int gridCounter = 0;
            int howManyRows = getHowManyRows();
            for (int y = 0; y < howManyRows; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    
                    Border brd = createBorder(x, y, (howManyRows-1));
                    Grid dayGrid = new Grid();
                    TextBlock txt = createTextBlock();
                    addCurrentDayMark(dayGrid, gridCounter);
                    txt.Text = daysForGridSetup[gridCounter].ToString();
                    gridCounter++;
                    dayGrid.Children.Add(txt);
                    brd.Child = dayGrid;
                    

                    (MonthViewGrid as Grid).Children.Add(brd);
                }
            }
        }

        private void addCurrentDayMark(Grid dayGrid, int gridCounter)
        {
            if (daysForGridSetupDateTimes[gridCounter].Date == DateTime.Now.Date)
            {
                Grid grid = new Grid();
                grid.Height = 10;
                grid.Width = 10;
                grid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                grid.Background = new SolidColorBrush(Colors.Red);
                dayGrid.Children.Add(grid);
            }
        }

        private int getHowManyRows()
        {
            int counter = 0;
            bool firstOneEncountered = false;
            foreach (int i in daysForGridSetup)
            {
                if (firstOneEncountered && i == 1)
                {
                    break;
                }
                if (i == 1)
                {
                    firstOneEncountered = true; 
                }
                counter++;
            }
            //return 6;
            return ((int)(counter / 7.0 + 1));
        }

       

        private static TextBlock createTextBlock()
        {
            TextBlock txt = new TextBlock();
            txt.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            txt.Margin = new Thickness(6);
            return txt;
        }


        private List<int> createDaysArray(DateTime forMonth)
        {

            List<int> days = new List<int>();
            daysForGridSetupDateTimes = new List<DateTime>();
            DateTime firstDay = forMonth.AddDays(-forMonth.Day).AddDays(1);
            int offsetBegin = 0;
            int lastDayOfPreviousMonth = firstDay.AddDays(-1).Day;
            int lastDayInThisMonth = DateTime.DaysInMonth(firstDay.Year, firstDay.Month); 

            switch (firstDay.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    offsetBegin = 0;
                    break;
                case DayOfWeek.Tuesday:
                    offsetBegin = 1;
                    break;
                case DayOfWeek.Wednesday:
                    offsetBegin = 2;
                    break;
                case DayOfWeek.Friday:
                    offsetBegin = 3;
                    break;
                case DayOfWeek.Thursday:
                    offsetBegin = 4;
                    break;
                case DayOfWeek.Saturday:
                    offsetBegin = 5;
                    break;
                case DayOfWeek.Sunday:
                    offsetBegin = 6;
                    break;

                default:
                    break;
            }

            for (int i = (offsetBegin - 1); i >= 0; i--)
            {
                int day = lastDayOfPreviousMonth - i;
                days.Add(day);
                daysForGridSetupDateTimes.Add(new DateTime(firstDay.Year, firstDay.AddMonths(-1).Month,day));
                
            }
            for (int i = 1; i <= lastDayInThisMonth; i++)
            {
                days.Add(i);
                daysForGridSetupDateTimes.Add(new DateTime(firstDay.Year, firstDay.Month , i));
            }

            for (int i = 1; i < 8; i++)
            {
                days.Add(i);
                daysForGridSetupDateTimes.Add(new DateTime(firstDay.Year, firstDay.AddMonths(+1).Month, i));
            }
            return days;
        }

        private static Border createBorder(int x, int y, int maxY)
        {
            Border brd = new Border();


            int leftThinkness = 1;
            int topThinkness = 1;
            int rightThinkness = 0;
            int bottomThinkness = 1;

            if (y != maxY)
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
            return brd;
        }
    }
}