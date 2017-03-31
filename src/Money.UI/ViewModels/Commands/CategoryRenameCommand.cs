using Money.Services;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

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

            if (viewModel.Key.IsEmpty)
            {
                Color color = Colors.Black;
                viewModel.Key = await domainFacade.CreateCategoryAsync(Name, color);
                viewModel.Name = Name;
                viewModel.Color = color;
            }
            else if (viewModel.Name != Name)
            {
                await domainFacade.RenameCategoryAsync(viewModel.Key, Name);
                viewModel.Name = Name;
            }

            if (viewModel.Description != Description)
            {
                await domainFacade.ChangeCategoryDescriptionAsync(viewModel.Key, Description);
                viewModel.Description = Description;
            }
        }
    }
}
