using Money.Services;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Money.Views
{
    [NavigationParameter(typeof(AboutParameter))]
    public sealed partial class About : Page
    {
        private readonly IFactory<ApplicationDataContainer> storageContainerFactory = ServiceProvider.StorageContainerFactory;
        private readonly IDevelopmentService developmentTools = ServiceProvider.DevelopmentTools;
        private readonly INavigator navigator = ServiceProvider.Navigator;

        public About()
        {
            InitializeComponent();
            ReloadException();

            DatabaseSwitch.IsOn = developmentTools.IsTestDatabaseEnabled();
        }

        private void ReloadException()
        {
            if (storageContainerFactory.Create().Containers.TryGetValue("Exception", out ApplicationDataContainer container))
            {
                ExceptionPanel.Visibility = Visibility.Visible;

                if (container.Values.TryGetValue("Type", out object type))
                    ExceptionType.Text = (string)type;

                if (container.Values.TryGetValue("Callstack", out object message))
                    ExceptionMessage.Text = (string)message;

                if (container.Values.TryGetValue("Callstack", out object callstack))
                    ExceptionCallstack.Text = (string)callstack;

                if (container.Values.TryGetValue("DateTime", out object dateTime))
                    ExceptionCallstack.Text = (string)dateTime;
            }
            else
            {
                ExceptionPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void ClearException_Click(object sender, RoutedEventArgs e)
        {
            storageContainerFactory.Create().DeleteContainer("Exception");
            ReloadException();
        }

        private void DatabaseSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (DatabaseSwitch.IsOn != developmentTools.IsTestDatabaseEnabled())
            {
                string state = DatabaseSwitch.IsOn ? "on" : "off";
                string message = $"Do you really want to turn test database {state}?{Environment.NewLine}{Environment.NewLine}No data will be lost and after switching back, you can continue where left off.{Environment.NewLine}{Environment.NewLine}Application needs a restart to complete the operation.";

                navigator
                    .Message(message)
                    .Button("Yes", new SwitchDatabaseCommand(developmentTools, DatabaseSwitch.IsOn))
                    .ButtonClose("No")
                    .Show();
            }
        }

        private class SwitchDatabaseCommand : Command
        {
            private readonly IDevelopmentService developmentTools;
            private readonly bool isEnabled;

            public SwitchDatabaseCommand(IDevelopmentService developmentTools, bool isEnabled)
            {
                Ensure.NotNull(developmentTools, "developmentTools");
                this.developmentTools = developmentTools;
                this.isEnabled = isEnabled;
            }

            public override bool CanExecute()
            {
                return true;
            }

            public override void Execute()
            {
                developmentTools.IsTestDatabaseEnabled(isEnabled);
                Application.Current.Exit();
            }
        }
    }
}
