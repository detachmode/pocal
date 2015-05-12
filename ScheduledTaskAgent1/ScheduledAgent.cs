//#define DEBUG_AGENT

using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

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


        protected override void OnInvoke(ScheduledTask task)
        {
            Deployment.Current.Dispatcher.BeginInvoke(async delegate
            {
                string msg;
                try
                {
                    LiveTileManager.AppointmentsOnLiveTile = await LiveTileManager.GetNextAppointments();
                    LiveTileManager.UpdateTile();

                    msg = "LiveTile geupdatet";
                }
                catch
                {
                    msg = "NOT UPDATED";
                }

                var toast = new ShellToast
                {
                    Title = "Pocal",
                    Content = msg
                };
                toast.Show();

#if DEBUG_AGENT
                ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
#endif

                NotifyComplete();
            });
        }
    }
}