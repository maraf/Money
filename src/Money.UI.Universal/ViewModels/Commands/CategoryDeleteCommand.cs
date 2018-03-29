using Money.Commands;
using Money.ViewModels.Navigation;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Commands
{
    public class CategoryDeleteCommand : Neptuo.Observables.Commands.Command
    {
        private readonly INavigator navigator;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IKey categoryKey;

        public CategoryDeleteCommand(INavigator navigator, ICommandDispatcher commandDispatcher, IKey categoryKey)
        {
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            Ensure.NotNull(categoryKey, "categoryKey");
            this.navigator = navigator;
            this.commandDispatcher = commandDispatcher;
            this.categoryKey = categoryKey;
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override void Execute()
        {
            navigator
                .Message("Do you really want to delete the category?")
                .Button("Yes", new ExecutiveCommand(commandDispatcher, categoryKey))
                .ButtonClose("No")
                .Show();
        }

        private class ExecutiveCommand : DomainCommand<DeleteCategory>
        {
            private readonly IKey categoryKey;

            public ExecutiveCommand(ICommandDispatcher commandDispatcher, IKey categoryKey)
                : base(commandDispatcher)
            {
                this.categoryKey = categoryKey;
            }

            public override bool CanExecute() => true;

            protected override DeleteCategory CreateDomainCommand() => new DeleteCategory(categoryKey);
        }
    }
}
