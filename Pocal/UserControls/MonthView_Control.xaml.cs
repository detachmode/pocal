using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.ApplicationModel.Appointments;
using Pocal.Helper;
using Pocal.Resources;
using Shared.Helper;

namespace Pocal
{
    public partial class MonthView_Control : UserControl
    {
        private int _gridCounter;
        private List<DateTime> _gridDateTimes;

        public MonthView_Control()
        {
            InitializeComponent();
        }

        public void LoadGridSetup(DateTime dt)
        {
            DayOfWeekNamesRow();
            GridSetup(dt);
            LoadAppointmentLinesAsync();
        }

        private void DayOfWeekNamesRow()
        {

            if (App.ViewModel.SettingsViewModel.FirstDayOfWeekIsSunday())
            {
                addWeekDayNameTextBlock(DayOfWeek.Sunday);
            }

            addWeekDayNameTextBlock(DayOfWeek.Monday);
            addWeekDayNameTextBlock(DayOfWeek.Tuesday);
            addWeekDayNameTextBlock(DayOfWeek.Wednesday);
            addWeekDayNameTextBlock(DayOfWeek.Thursday);
            addWeekDayNameTextBlock(DayOfWeek.Friday);
            addWeekDayNameTextBlock(DayOfWeek.Saturday);



            if (!App.ViewModel.SettingsViewModel.FirstDayOfWeekIsSunday())
            {
                addWeekDayNameTextBlock(DayOfWeek.Sunday);

            }

        }

        private void addWeekDayNameTextBlock(DayOfWeek dayOfWeek)
        {
            if (DateTimeFormatInfo.CurrentInfo != null)
            {
                var textBlock = new TextBlock
                {
                    Text = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(dayOfWeek),
                    Width = 68.5,
                    Padding = new Thickness(6.0),
                    Foreground = new SolidColorBrush(Colors.Gray)
                };
                DayOfWeekNamesRowStackPanel.Children.Add(textBlock);
            }
        }

        public async void LoadAppointmentLinesAsync()
        {
            var listOfAppointments =
                await CalendarAPI.GetAppointments(_gridDateTimes.FirstOrDefault(), _gridDateTimes.Count);
            foreach (var item in MonthViewGrid.Children)
            {
                var brd = item as Border;
                if (brd == null)
                    continue;

                var dtOfBrd = (DateTime) brd.DataContext;
                var appointmentsOfThisDay =
                    listOfAppointments.Where(x => TimeFrameChecker.IsInTimeFrameOfDay(x, dtOfBrd));
                AddAppointmentLines(appointmentsOfThisDay, brd);
            }
        }

        private static async void AddAppointmentLines(IEnumerable<Appointment> appointmentsOfThisDay, Border item)
        {
            var count = 1;
            var screenSizeMultiplicator = App.DisplayInformationEmulator.DisplayInformationEx.ViewPixelsPerHostPixel;

            foreach (var appt in appointmentsOfThisDay)
            {
                var pa = await App.ViewModel.CreatePocalAppoinment(appt);
                var rect = new Rectangle
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin =
                        new Thickness(6*screenSizeMultiplicator, 6*screenSizeMultiplicator*count,
                            6*screenSizeMultiplicator, 6*screenSizeMultiplicator),
                    Height = 3*screenSizeMultiplicator,
                    Width = 20*screenSizeMultiplicator,
                    Fill = pa.CalColor
                };

                var grid = item.Child as Grid;
                if (grid != null) grid.Children.Add(rect);
                count++;
            }
        }

        private static void DayTap(object sender, GestureEventArgs e)
        {
            var dt = (DateTime) ((FrameworkElement) sender).DataContext;
            ViewSwitcher.Mainpage.CloseMonthView();
            App.ViewModel.GoToDate(dt);
        }

        private void GridSetup(DateTime dt)
        {
            _gridCounter = 0;
            CreateDaysArray(dt.Date);

            var howManyRows = GetHowManyRows();
            for (var y = 0; y < howManyRows; y++)
            {
                for (var x = 0; x < 7; x++)
                {
                    var brd = CreateBorder(x, y, (howManyRows - 1));

                    var dayGrid = new Grid();


                    var txt = CreateTextBlock();
                    txt.Text = _gridDateTimes[_gridCounter].Day.ToString();
                    if (dt.Month != _gridDateTimes[_gridCounter].Month)
                    {
                        txt.Foreground = new SolidColorBrush(Colors.Gray);
                    }
                    AddMarkings(dayGrid);

                    _gridCounter++;
                    dayGrid.Children.Add(txt);
                    brd.Child = dayGrid;

                    MonthViewGrid.Children.Add(brd);
                }
            }
        }

        private void AddMarkings(Grid dayGrid)
        {
            if (_gridDateTimes[_gridCounter].Date == DateTime.Now.Date)
            {
                var grid =
                    CreateTriangleMark(new BitmapImage(new Uri(@"\Images\MonthViewDayNowMark.png", UriKind.Relative)), HorizontalAlignment.Left);
                dayGrid.Children.Add(grid);
            }

            if (_gridDateTimes[_gridCounter].Date == App.ViewModel.DayAtPointer.Dt.Date)
            {
                var grid =
                    CreateTriangleMark(new BitmapImage(new Uri(@"\Images\MonthViewDeltaTimeMark.png", UriKind.Relative)), HorizontalAlignment.Right);
                dayGrid.Children.Add(grid);
            }
        }

        private static Grid CreateTriangleMark(BitmapImage imageSource, HorizontalAlignment horizontalAlignment)
        {
            var grid = new Grid
            {
                Height = 20,
                Width = 20,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = horizontalAlignment
            };
            Canvas.SetZIndex(grid, 10);

            var ib = new ImageBrush
            {
                ImageSource = imageSource
            };

            grid.Background = ib;
            return grid;
        }

        private int GetHowManyRows()
        {
            var counter = 0;
            var firstOneEncountered = false;
            foreach (var d in _gridDateTimes)
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
            return ((int) (counter/7.0 + 1));
        }

        private static TextBlock CreateTextBlock()
        {
            var txt = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(6)
            };
            return txt;
        }

        private void CreateDaysArray(DateTime forMonth)
        {
            _gridDateTimes = new List<DateTime>();
            DateTime firstDay = forMonth.AddDays(-forMonth.Day).AddDays(1);
           
            DateTime lastDayOfPreviousMonth = firstDay.AddDays(-1);
            int lastDayInThisMonth = DateTime.DaysInMonth(firstDay.Year, firstDay.Month);
            DateTime firstDayInNextMonth = new DateTime(firstDay.Year, firstDay.Month, lastDayInThisMonth).AddDays(1);

            int offsetBegin = OffsetBeginOfWeek(firstDay);

            for (var i = (offsetBegin - 1); i >= 0; i--)
            {
                _gridDateTimes.Add(lastDayOfPreviousMonth.AddDays(-i));
            }
            for (var i = 1; i <= lastDayInThisMonth; i++)
            {
                _gridDateTimes.Add(new DateTime(firstDay.Year, firstDay.Month, i));
            }

            for (var i = 0; i < 7; i++)
            {
                _gridDateTimes.Add(firstDayInNextMonth.AddDays(i));
            }
        }

        private static int OffsetBeginOfWeek(DateTime firstDay)
        {
            int offsetBegin = 0;
            if (App.ViewModel.SettingsViewModel.FirstDayOfWeekIsSunday())
            {
                switch (firstDay.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        offsetBegin = 0;
                        break;
                    case DayOfWeek.Monday:
                        offsetBegin = 1;
                        break;
                    case DayOfWeek.Tuesday:
                        offsetBegin = 2;
                        break;
                    case DayOfWeek.Wednesday:
                        offsetBegin = 3;
                        break;
                    case DayOfWeek.Thursday:
                        offsetBegin = 4;
                        break;
                    case DayOfWeek.Friday:
                        offsetBegin = 5;
                        break;
                    case DayOfWeek.Saturday:
                        offsetBegin = 6;
                        break;
                }
            }
            else
            {

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
                }

            }
            return offsetBegin;
        }

        private Border CreateBorder(int x, int y, int maxY)
        {
            var brd = new Border();


            var leftThinkness = 1;
            const int topThinkness = 1;
            const int rightThinkness = 0;
            var bottomThinkness = 1;

            if (y != maxY)
                bottomThinkness = 0;

            if (x == 0)
                leftThinkness = 0;

            brd.BorderThickness = new Thickness(leftThinkness, topThinkness, rightThinkness, bottomThinkness);
            brd.VerticalAlignment = VerticalAlignment.Top;
            brd.HorizontalAlignment = HorizontalAlignment.Left;
            brd.BorderBrush = new SolidColorBrush((Color) Application.Current.Resources["PhoneBorderColor"]);

            var testDayOfWeek = _gridDateTimes[_gridCounter].DayOfWeek;
            if (testDayOfWeek == DayOfWeek.Saturday || testDayOfWeek == DayOfWeek.Sunday)
            {
                //brd.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 40, 40, 40));
                brd.Background = new SolidColorBrush((Color) Application.Current.Resources["MonthWeekendBg"]);
            }
            else
            {
                brd.Background = new SolidColorBrush((Color) Application.Current.Resources["MonthNoWeekendBg"]);
                //brd.Background = new SolidColorBrush();
            }


            brd.DataContext = _gridDateTimes[_gridCounter];
            brd.Tap += DayTap;

            const double height = (483/7.0);
            const double width = (483/7.0);
            brd.Height = height;
            brd.Width = width;

            var leftmargin = height*x;
            var topmargin = width*y;
            brd.Margin = new Thickness(leftmargin, topmargin, 0, 0);
            return brd;
        }
    }
}