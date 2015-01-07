using System;
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
using Pocal.Converter;
using System.Globalization;
using Pocal.Resources;

namespace Pocal
{
    public partial class SDV_Control : UserControl
    {
        internal int FirstHour = 0;
        private int LastHour = 24;

        public SDV_Control()
        {
            InitializeComponent();
            GridSetup();
        }


        public void GridSetup()
        {
            for (int i = FirstHour; i < LastHour; i++)
            {
                string str = "";
                if (AppResources.ResourceLanguage.Contains("en"))
                    str = convert12(i);
                else
                    str = i.ToString("00") + ":00"; 

                SDV_HourLine_Control control = new SDV_HourLine_Control(str);
                DateTime dt = new DateTime(1, 1, 1, i, 0, 0);
                control.DataContext = new HourLine { DT = dt};
                StackpanelHourLines.Children.Add(control);

            }
        }

        private string convert12(int i)
        {
            if (i == 0 )
            {
                return "12 AM";
            }
            if (i <= 11)
            {
                return (i).ToString() + " AM"; 
            }
            if (i == 12)
            {
                return "12 PM"; 
            }
            return (i%12).ToString() + " PM";
        }


        public void PrepareForNewLoadingOfAppoinments()
        {
            GridAppointments.Children.Clear();
            //cleanScrollviewer();
        }

        private void cleanScrollviewer()
        {
            SDV_ViewportControl.ClearValue(ScrollViewer.DataContextProperty);
            SDV_ViewportControl.Measure(new Size(0, 0));
            SDV_ViewportControl.Arrange(new Rect(0, 0, 0, 0));
        }


        public void AddTappedDayAppointments()
        {
            if (App.ViewModel.SingleDayViewModel.TappedDay == null)
                return;

            foreach (PocalAppointment pa in App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay)
            {
                if (!pa.AllDay)
                {
                    SDV_Appointment_Control control = new SDV_Appointment_Control();
                    control.DataContext = pa;
                    GridAppointments.Children.Add(control);
                }
            }

        }

        public void Update_PocalAppointment(PocalAppointment oldPA, PocalAppointment newPA)
        {
            for (int i = GridAppointments.Children.Count-1; i >= 0 ; i--)
            {
                FrameworkElement item = GridAppointments.Children[i] as FrameworkElement;
                if (item.DataContext == oldPA)
                    GridAppointments.Children.Remove(item); 
            }

            if (newPA != null && newPA.AllDay == false && newPA.StartTime.Date == App.ViewModel.SingleDayViewModel.TappedDay.DT.Date)
            {
                //fixme
                newPA = App.ViewModel.SingleDayViewModel.TappedDay.PocalApptsOfDay.FirstOrDefault(x => x.Appt.LocalId == newPA.Appt.LocalId);
                SDV_Appointment_Control control = new SDV_Appointment_Control();
                control.DataContext = newPA;
                GridAppointments.Children.Add(control);
            }
            

        }

    }
}
