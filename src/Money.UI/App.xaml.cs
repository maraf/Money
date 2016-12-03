using Money.Bootstrap;
using Money.Services;
using Money.ViewModels;
using Money.ViewModels.Parameters;
using Money.Views;
using Money.Views.Navigation;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Money.UI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public IDomainFacade DomainFacade { get; private set; }
        public INavigator Navigator { get; set; }

        public static new App Current
        {
            get { return (App)Application.Current; }
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            //if (Debugger.IsAttached)
            //{
            //    this.DebugSettings.EnableFrameRateCounter = true;
            //}
#endif
            Bootstrap();

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            Navigator = new ApplicationNavigator(new NavigatorParameterCollection(), rootFrame);

            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested += OnBackRequested;

            rootFrame.Navigating += OnRootFrameNavigating;
            rootFrame.Navigated += OnRootFrameNavigated;

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    //rootFrame.Navigate(typeof(GroupPage), GroupType.Month);
                    Navigator
                        .Open(new SummaryParameter() { Month = DateTime.Now })
                        .Show();
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        private void Bootstrap()
        {
            BootstrapTask task = new BootstrapTask();
            task.Initialize();

            //Outcome outcome = new Outcome(task.PriceFactory.Create(2500), "This is my first outcome", DateTime.Now);
            //IKey outcomeKey = outcome.Key;
            //task.OutcomeRepository.Save(outcome);

            //outcome = task.OutcomeRepository.Find(outcomeKey);
            //Debug.WriteLine($"Outcome of '{outcome.Amount}' with description '{outcome.Description}' from '{outcome.When}'.");
            DomainFacade = task.DomainFacade;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            Frame frame = (Frame)sender;
            EnsureBackButtonVisibility((Frame)sender);

            Page page = frame.Content as Page;
            if (page != null)
                page.PointerPressed += OnPagePointerPressed;
        }

        private void OnRootFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            Frame frame = (Frame)sender;

            Page page = frame.Content as Page;
            if (page != null)
                page.PointerPressed -= OnPagePointerPressed;
        }

        private void OnPagePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                Page page = (Page)sender;
                PointerPoint point = e.GetCurrentPoint(page);
                if (point.Properties.IsXButton1Pressed)
                    NavigateBack(page.Frame);
                else if (point.Properties.IsXButton2Pressed)
                    NavigateForward(page.Frame);
            }
        }

        private void EnsureBackButtonVisibility(Frame rootFrame)
        {
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.AppViewBackButtonVisibility = rootFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame frame = sender as Frame;
            if (frame == null)
                frame = Window.Current.Content as Frame;

            if (frame == null)
                return;

            if (NavigateBack(frame))
                e.Handled = true;
        }

        private bool NavigateBack(Frame rootFrame)
        {
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack(new DrillInNavigationTransitionInfo());
                return true;
            }

            return false;
        }

        private bool NavigateForward(Frame rootFrame)
        {
            if (rootFrame.CanGoForward)
            {
                rootFrame.GoForward();
                return true;
            }

            return false;
        }
    }
}
