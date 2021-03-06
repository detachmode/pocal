﻿
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Pocal.Resources;
using Pocal.ViewModel;
using ScheduledTaskAgent1;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ScreenSizeSupport;
using Microsoft.Phone.Tasks;
using System.Linq;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Marketplace;
using System.Globalization;


namespace Pocal
{
    public partial class App : Application
    {

        private static MainViewModel viewModel = null;
        public static MainViewModel ViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (viewModel == null)
                    viewModel = new MainViewModel();

                return viewModel;
            }
        }
        public static DisplayInformationEmulator DisplayInformationEmulator { get; set; }

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {


            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage();

            DisplayInformationEmulator = Resources["DisplayInformationEmulator"] as DisplayInformationEmulator;

            if (!DesignerProperties.IsInDesignTool)
                UserSettings = IsolatedStorageSettings.ApplicationSettings;

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;


                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        public static void LoadMyRessources()
        {
            App.Current.Resources.Remove("AgendaBg");
            App.Current.Resources.Remove("SdvBg");
            App.Current.Resources.Remove("MonthBg");
            App.Current.Resources.Remove("MonthWeekendBg");
            App.Current.Resources.Remove("MonthNoWeekendBg");
            App.Current.Resources.Remove("AgendaPointerImage");

            Visibility v = (Visibility)App.Current.Resources["PhoneLightThemeVisibility"];

            if (v == System.Windows.Visibility.Visible)
            {
                //LIGHT
                App.Current.Resources.Add("AgendaBg", Colors.White);

                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri(@"\Images\AgendaPointerBright.png", UriKind.Relative));
                ib.Stretch = Stretch.None;
                App.Current.Resources.Add("AgendaPointerImage", ib);


                App.Current.Resources.Add("SdvBg", Color.FromArgb(255, 240, 240, 240));
                App.Current.Resources.Add("MonthBg", Color.FromArgb(255, 240, 240, 240));
                App.Current.Resources.Add("MonthWeekendBg", Color.FromArgb(255, 230, 230, 230));
                App.Current.Resources.Add("MonthNoWeekendBg", Colors.White);
                //App.Current.Resources["SDV_BG"] = Colors.Magenta;
            }
            else
            {
                //DARK
                App.Current.Resources.Add("AgendaBg", Colors.Black);

                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri(@"\Images\AgendaPointerDark.png", UriKind.Relative));
                ib.Stretch = Stretch.None;
                App.Current.Resources.Add("AgendaPointerImage", ib);

                App.Current.Resources.Add("SdvBg", Color.FromArgb(255, 15, 15, 15));
                App.Current.Resources.Add("MonthBg", Colors.Black);
                App.Current.Resources.Add("MonthWeekendBg", Color.FromArgb(255, 30, 30, 30));
                App.Current.Resources.Add("MonthNoWeekendBg", Colors.Black);
            }

            if (AppResources.ResourceLanguage.Contains("en"))
            {
                ViewModel.SettingsViewModel.ChoosenTimeStyleDefault = 1;
            }

            SetResourcesForTimestyle();
        }

        public static void SetResourcesForTimestyle()
        {
            if (ViewModel.SettingsViewModel.IsTimeStyleAMPM())
            {
                App.Current.Resources.Remove("OverviewStartTimeX");
                App.Current.Resources.Add("OverviewStartTimeX", 40);
                App.Current.Resources.Remove("OverviewStartTimeWidth");
                App.Current.Resources.Add("OverviewStartTimeWidth", 90);
            }
            else
            {
                App.Current.Resources.Remove("OverviewStartTimeX");
                App.Current.Resources.Add("OverviewStartTimeX", 20);
                App.Current.Resources.Remove("OverviewStartTimeWidth");
                App.Current.Resources.Add("OverviewStartTimeWidth", 60);
            }
        }

        // Code to execute when a contract activation such as a file open or save picker returns 
        // with the picked file or other return values
        private void Application_ContractActivated(object sender, Windows.ApplicationModel.Activation.IActivatedEventArgs e)
        {
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            //CheckLicense();
            //ShowBuyPopUpIfTrial();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            //CheckLicense();
            LiveTileManager.UpdateTileFromForeground();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Ensure that required application state is persisted here.
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Handle contract activation such as a file open or save picker
            PhoneApplicationService.Current.ContractActivated += Application_ContractActivated;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {

                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }


        public static MarketplaceDetailTask _marketPlaceDetailTask = new MarketplaceDetailTask();
        public static IsolatedStorageSettings UserSettings;
        private static LicenseInformation _licenseInformation = new LicenseInformation();


        public static void ShowTrialPopUp()
        {
            bool isTrail = _licenseInformation.IsTrial();

            // comment out before release!!
            //isTrail = true;

            if (isTrail)
            {
                try
                {
                    int trailDaysLeft = getTrialDaysLeft();
                    string message = String.Format(AppResources.trialPopup, trailDaysLeft);
                    if (MessageBox.Show(message, "Trial", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        App._marketPlaceDetailTask.Show();
                        IsolatedStorageSettings.ApplicationSettings.Save();
                        Application.Current.Terminate();
                    }
                    else
                    {
                        if (trailDaysLeft == 0)
                        {
                            IsolatedStorageSettings.ApplicationSettings.Save();
                            Application.Current.Terminate();
                        }
                    }

                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    UserSettings.Add("installDate", DateTime.Now.Date);
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
            }
        }

        private static int getTrialDaysLeft()
        {
            DateTime installDate = (DateTime)UserSettings["installDate"];
            int trailDaysLeft = (installDate.Date.AddDays(7) - DateTime.Now.Date).Days;
            if (trailDaysLeft < 0)
                trailDaysLeft = 0;
            return trailDaysLeft;
        }
    }
}