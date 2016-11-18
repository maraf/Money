using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

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
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OutcomeViewModel.Amount))
                RaiseCanExecuteChanged();
        }

        public override bool CanExecute()
        {
            return viewModel.Amount > 0;
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
