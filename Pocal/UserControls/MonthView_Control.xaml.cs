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
using Pocal.ViewModel;
using System.Windows.Media.Imaging;

namespace Pocal
{
    public partial class MonthView_Control : UserControl
    {
        public MonthView_Control()
        {
            InitializeComponent();
        }

        public void loadGridSetup(DateTime dt)
        {


            gridSetup(dt);

        }

        void dayTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            DateTime dt = (DateTime)((FrameworkElement)sender).DataContext;
            ViewSwitcher.mainpage.CloseMonthView();
            App.ViewModel.GoToDate(dt);

        }


        private List<DateTime> gridDateTimes;
        int gridCounter;
        private void gridSetup(DateTime dt)
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
                    
                    //addDeltaDayMark(dayGrid);

                    TextBlock txt = createTextBlock();
                    txt.Text = gridDateTimes[gridCounter].Day.ToString();
                    if (dt.Month != gridDateTimes[gridCounter].Month)
                    {
                        txt.Foreground = new SolidColorBrush(Colors.Gray);
                    }
                    addMarkings(dayGrid);

                    gridCounter++;
                    dayGrid.Children.Add(txt);
                    brd.Child = dayGrid;

                    (MonthViewGrid as Grid).Children.Add(brd);
                }
            }
        }

        private void addMarkings(Grid dayGrid)
        {
            if (gridDateTimes[gridCounter].Date == DateTime.Now.Date)
            {
                Grid grid = new Grid();
                grid.Height = 20;
                grid.Width = 20;
                grid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri(@"\Images\MonthViewDayNowMark.png", UriKind.Relative));
                grid.Background = ib;                    


                dayGrid.Children.Add(grid);
            }
            if (gridDateTimes[gridCounter].Date == App.ViewModel.DayAtPointer.DT.Date)
            {
                Grid grid = new Grid();
                grid.Height = 20;
                grid.Width = 20;
                grid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri(@"\Images\MonthViewDeltaTimeMark.png", UriKind.Relative));
                grid.Background = ib;


                dayGrid.Children.Add(grid);
            }
        }

        private void addDeltaDayMark(Grid dayGrid)
        {
            if (gridDateTimes[gridCounter].Date == App.ViewModel.DayAtPointer.DT.Date)
            {
                Grid grid = new Grid();
                grid.Height = 25;
                grid.Width = 25;
                grid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri(@"\Images\MonthViewDeltaTimeMark.png", UriKind.Relative));
                grid.Background = ib;


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
            brd.Tap += dayTap;

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
