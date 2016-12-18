using Money.Services;
using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Money.ViewModels.Commands
{
    public class CategoryChangeColorCommand : Command
    {
        private readonly IDomainFacade domainFacade;
        private readonly CategoryEditViewModel viewModel;

        public Color Color { get; set; }

        public CategoryChangeColorCommand(IDomainFacade domainFacade, CategoryEditViewModel viewModel)
        {
            Ensure.NotNull(domainFacade, "domainFacade");
            Ensure.NotNull(viewModel, "viewModel");
            this.domainFacade = domainFacade;
            this.viewModel = viewModel;

            Color = viewModel.Color;
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override async void Execute()
        {
            if (Color != viewModel.Color)
            {
                await domainFacade.ChangeCategoryColor(viewModel.Key, Color);
                viewModel.Color = Color;
            }
        }
    }
}
