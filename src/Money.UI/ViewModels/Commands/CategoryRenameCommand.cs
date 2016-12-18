using Money.Services;
using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Commands
{
    public class CategoryRenameCommand : Command
    {
        private readonly IDomainFacade domainFacade;
        private readonly CategoryEditViewModel viewModel;

        public string Name { get; set; }
        public string Description { get; set; }

        public CategoryRenameCommand(IDomainFacade domainFacade, CategoryEditViewModel viewModel)
        {
            Ensure.NotNull(domainFacade, "domainFacade");
            Ensure.NotNull(viewModel, "viewModel");
            this.domainFacade = domainFacade;
            this.viewModel = viewModel;

            Name = viewModel.Name;
            Description = viewModel.Description;
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override async void Execute()
        {
            if (String.IsNullOrEmpty(Name))
                return;

            if (viewModel.Name != Name)
                await domainFacade.RenameCategory(viewModel.Key, Name);

            if (viewModel.Description != Description)
                await domainFacade.ChangeCategoryDescription(viewModel.Key, Description);

            viewModel.Name = Name;
            viewModel.Description = Description;
        }
    }
}
