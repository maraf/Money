using Money.Bootstrap;
using Money.Services;
using Money.ViewModels;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views;
using Money.Views.Navigation;
using Neptuo.Exceptions.Handlers;
using Neptuo.Models;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
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
    sealed partial class App : Application, IExceptionHandler
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
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
            TryBootstrap(e.Arguments);

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();    
                statusBar.HideAsync();
            }

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content, just ensure that the window is active
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
                ServiceProvider.Navigator = new AppNavigator(new NavigatorParameterCollection(), rootFrame);
            }

            // This one is for migration designing.
            //ServiceProvider.UpgradeService = new Views.DesignData.TestUpgradeService();

            if (e.PrelaunchActivated == false)
            {
                object parameter;
                if (ServiceProvider.TileService.TryParseNavigation(e, out parameter))
                {
                    ServiceProvider.Navigator
                        .Open(parameter)
                        .Show();
                }
                else if (rootFrame.Content == null)
                {
                    if (ServiceProvider.UpgradeService.IsRequired())
                    {
                        ServiceProvider.Navigator
                            .Open(new MigrateParameter())
                            .Show();
                    }
                    else
                    {
                        ServiceProvider.Navigator
                            .Open(new SummaryParameter())
                            .Show();
                    }
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        public Bootstrap.BootstrapTask BootstrapTask { get; private set; }

        private bool TryBootstrap(string launchArguments)
        {
            if (BootstrapTask == null)
            {
                BootstrapTask = new Bootstrap.BootstrapTask(this, launchArguments);
                BootstrapTask.Initialize();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e) => throw new Exception("Failed to load Page " + e.SourcePageType.FullName);

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

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            if (ProcessException(e.Exception))
                e.Handled = true;
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            if (ProcessException(e.Exception))
                e.SetObserved();
        }

        void IExceptionHandler.Handle(Exception exception) 
            => CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => ProcessException(exception));

        private bool ProcessException(Exception e)
        {
            try
            {
                return ProcessExceptionInernal(e);
            }
            catch(Exception ex)
            {
                MessageDialog dialog = new MessageDialog(ex.ToString(), "Fatal Error");
                dialog.ShowAsync();
                return true;
            }
        }

        private bool ProcessExceptionInernal(Exception e)
        {
            INavigator navigator = ServiceProvider.Navigator;
            if (e is AggregateRootException && navigator != null)
            {
                string message = null;

                MessageBuilder messageBuilder = ServiceProvider.MessageBuilder;
                if (e is CurrencyAlreadyAsDefaultException)
                    message = messageBuilder.CurrencyAlreadyAsDefault();
                else if (e is CurrencyAlreadyExistsException)
                    message = messageBuilder.CurrencyAlreadyExists();
                else if (e is CurrencyDoesNotExistException)
                    message = messageBuilder.CurrencyDoesNotExist();
                else if (e is CurrencyExchangeRateDoesNotExistException)
                    message = messageBuilder.CurrencyExchangeRateDoesNotExist();
                else if (e is OutcomeAlreadyDeletedException)
                    message = messageBuilder.OutcomeAlreadyDeleted();
                else if (e is OutcomeAlreadyHasCategoryException)
                    message = messageBuilder.OutcomeAlreadyHasCategory();
                else if (e is CantDeleteDefaultCurrencyException)
                    message = messageBuilder.CantDeleteDefaultCurrency();
                else if (e is CantDeleteLastCurrencyException)
                    message = messageBuilder.CantDeleteLastCurrency();


                if (message != null)
                {
                    navigator
                        .Message(message, "Error")
                        .Show();

                    return true;
                }
            }

            ApplicationDataContainer container = ServiceProvider.StorageContainerFactory.Create()
                .CreateContainer("Exception", ApplicationDataCreateDisposition.Always);

            container.Values["Type"] = e.GetType().FullName;
            container.Values["Message"] = SubstringValueForContainer(e.Message);
            container.Values["Callstack"] = SubstringValueForContainer(e.StackTrace + e.InnerException?.StackTrace);
            container.Values["DateTime"] = DateTime.Now.ToString();

#if DEBUG
            if (Debugger.IsAttached)
                Debugger.Break();

            object content = Window.Current?.Content;
            if (content is Frame frame)
                content = frame.Content;

            if (content is ITemplate template)
                template.HideLoading();

            if (e is LayoutCycleException)
                Exit();

            if (navigator != null)
            {
                navigator
                    .Message(e.ToString(), "Exception")
                    .Show();

                return true;
            }
#endif

            return false;
        }

        private static string SubstringValueForContainer(string value)
        {
            const int maxLength = 255;

            if (String.IsNullOrEmpty(value))
                return value;

            if (value.Length > maxLength)
                return value.Substring(0, maxLength);

            return value;
        }
    }
}
