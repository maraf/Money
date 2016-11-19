using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Specialized;
using Money.Services;

namespace Money.ViewModels.Commands
{
    public class SaveOutcomeCommand : Command
    {
        private readonly OutcomeViewModel viewModel;
        private readonly IDomainFacade domainFacade;

        public SaveOutcomeCommand(OutcomeViewModel viewModel, IDomainFacade domainFacade)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(domainFacade, "domainFacade");
            this.viewModel = viewModel;
            this.viewModel.PropertyChanged += OnViewModelPropertyChanged;
            this.viewModel.SelectedCategories.CollectionChanged += OnViewModelSelectedCategoriesChanged;
            this.domainFacade = domainFacade;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OutcomeViewModel.Amount))
                RaiseCanExecuteChanged();
        }

        private void OnViewModelSelectedCategoriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        public override bool CanExecute()
        {
            return viewModel.Amount > 0 && viewModel.SelectedCategories.Count > 0;
        }

        public override async void Execute()
        {
            await domainFacade.CreateOutcomeAsync(
                domainFacade.PriceFactory.Create((decimal)viewModel.Amount), 
                viewModel.Description, 
                viewModel.When
            );

            viewModel.Amount = 0;
            viewModel.Description = null;
            viewModel.When = DateTime.Now;
        }
    }
}
