using Money.UI;
using Money.ViewModels;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo.Migrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Money.Views
{
    [NavigationParameter(typeof(MigrateParameter))]
    public sealed partial class Migrate : Page
    {
        private readonly IUpgradeService upgradeService = ServiceProvider.UpgradeService;

        public MigrateViewModel ViewModel
        {
            get { return (MigrateViewModel)DataContext; }
            set { DataContext = value; }
        }

        public Migrate()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (upgradeService.IsRequired())
            {
                ViewModel = new MigrateViewModel(upgradeService);

                await ViewModel.StartAsync();
            }

            ServiceProvider.Navigator
                .Open(new SummaryParameter())
                .Show();
        }
    }
}
