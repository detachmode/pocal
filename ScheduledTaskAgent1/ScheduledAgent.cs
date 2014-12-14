#define DEBUG_AGENT

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
        /// <remarks>
        /// ScheduledAgent-Konstruktor, initialisiert den UnhandledException-Handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Handler für verwaltete Ausnahmen abonnieren
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code, der bei nicht behandelten Ausnahmen ausgeführt wird
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // Eine nicht behandelte Ausnahme ist aufgetreten. Unterbrechen und Debugger öffnen
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent zum Ausführen einer geplanten Aufgabe
        /// </summary>
        /// <param name="task">
        /// Die aufgerufene Aufgabe
        /// </param>
        /// <remarks>
        /// Diese Methode wird aufgerufen, wenn eine regelmäßige oder ressourcenintensive Aufgabe aufgerufen wird
        /// </remarks>
        /// 

        protected async override void OnInvoke(ScheduledTask task)
        {
            LiveTileManager.AppointmentOnLiveTile = await LiveTileManager.getNextAppointment();
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                //TODO: Add code to perform your task in background
                string toastMessage = "";

                // Execute resource-intensive task actions here.
                toastMessage = "Resource-intensive task running.";


                // Launch a toast to show that the agent is running.
                // The toast will not be shown if the foreground application is running.
                ShellToast toast = new ShellToast();
                toast.Title = "Background Agent Sample";
                toast.Content = toastMessage;
                toast.Show();
                LiveTileManager.UpdateTile();


                // If debugging is enabled, launch the agent again in one minute.
#if DEBUG_AGENT
                ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
#endif

                // Call NotifyComplete to let the system know the agent is done working.
                NotifyComplete();
            });
        }


        //protected override void OnInvoke(ScheduledTask task)
        //{

        //Deployment.Current.Dispatcher.BeginInvoke(delegate
        //{

        //        ShellToast toast = new ShellToast();
        //        toast.Title = "Pocal";
        //        toast.Content = "Updated LiveTile";
        //        toast.Show();
        //        LiveTileManager.UpdateTile();

        //        NotifyComplete();
        //    });

        //}
    }
}