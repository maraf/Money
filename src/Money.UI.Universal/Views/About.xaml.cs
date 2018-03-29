using Money.Services;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo;
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
        private readonly IDevelopmentService developmentTools = ServiceProvider.DevelopmentTools;
        private readonly INavigator navigator = ServiceProvider.Navigator;

        public About()
        {
            InitializeComponent();

            DatabaseSwitch.IsOn = developmentTools.IsTestDatabaseEnabled();
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

        private class SwitchDatabaseCommand : Neptuo.Observables.Commands.Command
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
