using Money.ViewModels;
using Money.ViewModels.Parameters;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Money.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OutcomePage : Page
    {
        public OutcomePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            OutcomeViewModel viewModel = null;

            Guid? id = e.Parameter as Guid?;
            if (id != null)
            {
                // TODO: Load existing.
                viewModel = new OutcomeViewModel(id.Value);
            }
            else
            {
                OutcomeDefaultsModel defaults = e.Parameter as OutcomeDefaultsModel;
                if (defaults != null)
                {
                    viewModel = new OutcomeViewModel(defaults);
                }
                else
                {
                    viewModel = new OutcomeViewModel();
                }
            }

            viewModel.Categories.AddRange(new DesignData.ViewModelLocator().CreateOutcome.Categories);
            DataContext = viewModel;
        }
    }
}
