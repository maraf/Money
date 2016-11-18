using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Money.ViewModels.Commands
{
    public class SaveOutcomeCommand : Command
    {
        private readonly OutcomeViewModel viewModel;

        public SaveOutcomeCommand(OutcomeViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            this.viewModel = viewModel;
            this.viewModel.PropertyChanged += OnViewModelPropertyChanged;
            this.viewModel.SelectedCategories.CollectionChanged += OnViewModelSelectedCategoriesChanged;
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

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
