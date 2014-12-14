//#define DEBUG_AGENT

using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using Windows.ApplicationModel.Appointments;

namespace ScheduledTaskAgent1
{
    public class ScheduledAgent : ScheduledTaskAgent
    {

        static ScheduledAgent()
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }


        protected async override void OnInvoke(ScheduledTask task)
        {
            LiveTileManager.AppointmentOnLiveTile = await LiveTileManager.getNextAppointment();
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {

                //string toastMessage = "";
                //toastMessage = "LiveTile geupdatet.";
                //ShellToast toast = new ShellToast();
                //toast.Title = "Pocal";
                //toast.Content = toastMessage;
                //toast.Show();

                LiveTileManager.UpdateTile();

#if DEBUG_AGENT
                ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
#endif

                
                NotifyComplete();
            });
        }

    }
}