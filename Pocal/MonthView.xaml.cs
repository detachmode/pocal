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

namespace Pocal
{
    public partial class MonthView : PhoneApplicationPage
    {
      
        public MonthView()
        {
            this.DataContext = App.ViewModel.MonthViewModel;

            InitializeComponent();

            

            //createPivots();
            



        }

        private void createPivots()
        {
            for (int i = 0; i < 6; i++)
            {
                if (i == 5)
                    createInitialPivotItem(DateTime.Now.AddMonths(-1));
                else
                    createInitialPivotItem(DateTime.Now.AddMonths(i));
            }


        }



        private void createInitialPivotItem(DateTime dt)
        {
            PivotItem pi = new PivotItem();
            pi.Loaded += pi_Loaded;
            pi.Margin = new Thickness(0, 0, 0, 0);
            pi.Header = dt.ToString("MMMMMM", cultureSettings.ci);
            pi.DataContext = dt;

            Grid monthViewGrid = new Grid();
            monthViewGrid.Height = 480;
            monthViewGrid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            monthViewGrid.Margin = new Thickness(0, 70, 0, 70);
            //monthViewGrid.Background = new SolidColorBrush(Colors.Blue);

            StackPanel stack = new StackPanel();
            stack.Orientation = System.Windows.Controls.Orientation.Horizontal;
            stack.Height = 30;
            stack.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            stack.Margin = new Thickness(0, -30, 0, 0);

            string[] weekNames = { "Mo", "Di", "Mi", "Do", "Fr", "Sa", "So" };


            foreach (string str in weekNames)
            {
                TextBlock txt = new TextBlock();
                txt.Width = 68.5;
                txt.Padding = new Thickness(6, 0, 6, 0);
                txt.Foreground = new SolidColorBrush(Colors.Gray);
                txt.Text = str;

                stack.Children.Add(txt);
            }

            monthViewGrid.Children.Add(stack);



            //monthViewGrid = gridSetup(monthViewGrid, dt);

            pi.Content = monthViewGrid;

            (MonthViewPivot as Pivot).Items.Add(pi);
        }

        void pi_Loaded(object sender, RoutedEventArgs e)
        {
           //throw new NotImplementedException();
        }



        //private List<int> daysForGridSetup;
        private List<DateTime> gridDateTimes;
        int gridCounter;

        private Grid gridSetup(Grid monthViewGrid, DateTime dt)
        {
            gridCounter = 0;
            createDaysArray(dt.Date);

            int howManyRows = getHowManyRows();
            for (int y = 0; y < howManyRows; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    Border brd = createBorder(x, y, (howManyRows - 1));
                    //brd.Tap += dayGrid_Tap;
                    Grid dayGrid = new Grid();


                    TextBlock txt = createTextBlock();

                    addCurrentDayMark(dayGrid);
                    txt.Text = gridDateTimes[gridCounter].Day.ToString();
                    gridCounter++;
                    dayGrid.Children.Add(txt);
                    brd.Child = dayGrid;

                    monthViewGrid.Children.Add(brd);
                }
            }
            return monthViewGrid;
        }




        private void addCurrentDayMark(Grid dayGrid)
        {
            if (gridDateTimes[gridCounter].Date == DateTime.Now.Date)
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
            foreach (var d in gridDateTimes)
            {
                if (firstOneEncountered && d.Day == 1)
                {
                    break;
                }
                if (d.Day == 1)
                {
                    firstOneEncountered = true;
                }
                counter++;
            }
            return ((int)(counter / 7.0 + 1));
        }



        private static TextBlock createTextBlock()
        {
            TextBlock txt = new TextBlock();
            txt.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            txt.Margin = new Thickness(6);
            return txt;
        }


        private void createDaysArray(DateTime forMonth)
        {
            gridDateTimes = new List<DateTime>();
            DateTime firstDay = forMonth.AddDays(-forMonth.Day).AddDays(1);
            int offsetBegin = 0;
            DateTime lastDayOfPreviousMonth = firstDay.AddDays(-1);
            int lastDayInThisMonth = DateTime.DaysInMonth(firstDay.Year, firstDay.Month);
            DateTime firstDayInNextMonth = new DateTime(firstDay.Year, firstDay.Month, lastDayInThisMonth).AddDays(1);

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
                case DayOfWeek.Thursday:
                    offsetBegin = 3;
                    break;
                case DayOfWeek.Friday:
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
                gridDateTimes.Add(lastDayOfPreviousMonth.AddDays(-i));

            }
            for (int i = 1; i <= lastDayInThisMonth; i++)
            {
                gridDateTimes.Add(new DateTime(firstDay.Year, firstDay.Month, i));
            }

            for (int i = 0; i < 7; i++)
            {
                gridDateTimes.Add(firstDayInNextMonth.AddDays(i));
            }

        }

        private Border createBorder(int x, int y, int maxY)
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

            var testDayOfWeek = gridDateTimes[gridCounter].DayOfWeek;
            if (testDayOfWeek == DayOfWeek.Saturday || testDayOfWeek == DayOfWeek.Sunday)
            {
                brd.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 40, 40, 40));
            }
            else
            {
                brd.Background = new SolidColorBrush(Colors.Black);
            }


            brd.DataContext = gridDateTimes[gridCounter];
            brd.Tap += brdTap;

            double height = (483 / 7);
            double width = (483 / 7);
            brd.Height = height;
            brd.Width = width;

            double leftmargin = height * x;
            double topmargin = width * y;
            brd.Margin = new Thickness(leftmargin, topmargin, 0, 0);
            return brd;
        }



        void brdTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            DateTime dt = (DateTime)((FrameworkElement)sender).DataContext;
            App.ViewModel.GoToDate(dt);
            NavigationService.Navigate(new Uri("/Mainpage.xaml?GoToDate=", UriKind.Relative), dt);

        }

        private void MonthViewPivot_LoadedPivotItem(object sender, PivotItemEventArgs e)
        {
            //if 
            //{
            //addOneMorePivotForward();


            PivotItem pi = new PivotItem();
            //pi.Loaded += pi_Loaded;
            pi.Margin = new Thickness(0, 0, 0, 0);
            pi.Header = "test";
            (MonthViewPivot as Pivot).Items.Add(pi);
            //pi.DataContext = dt;
            //}
        }

        private void addOneMorePivotForward()
        {
            DateTime latestMonth = (DateTime)((FrameworkElement)MonthViewPivot.Items.ElementAt(MonthViewPivot.Items.Count - 2)).DataContext;
            createInitialPivotItem(latestMonth.AddMonths(1));
        }

        private void MonthViewPivot_Loaded_1(object sender, RoutedEventArgs e)
        {
            MonthViewPivot.UpdateLayout();
            MonthViewPivot.LoadedPivotItem += MonthViewPivot_LoadedPivotItem;
        }






    }
}