using Money.Services;
using Money.ViewModels.Navigation;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Commands
{
    public class SaveOutcomeCommand  : NavigateBackCommand
    {
        private readonly OutcomeViewModel viewModel;
        private readonly ICommandDispatcher commandDispatcher;

        public SaveOutcomeCommand(INavigator navigator, OutcomeViewModel viewModel, ICommandDispatcher commandDispatcher)
            : base(navigator)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            this.viewModel = viewModel;
            this.viewModel.PropertyChanged += OnViewModelPropertyChanged;
            this.viewModel.SelectedCategories.CollectionChanged += OnViewModelSelectedCategoriesChanged;
            this.commandDispatcher = commandDispatcher;
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
            throw Ensure.Exception.NotImplemented();
            
            base.Execute();
        }
    }
}
