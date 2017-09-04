using Money.ViewModels.Navigation;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Commands
{
    public class CategoryDeleteCommand : Command
    {
        private readonly INavigator navigator;
        private readonly IDomainFacade domainFacade;
        private readonly IKey categoryKey;

        public CategoryDeleteCommand(INavigator navigator, IDomainFacade domainFacade, IKey categoryKey)
        {
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(domainFacade, "domainFacade");
            Ensure.NotNull(categoryKey, "categoryKey");
            this.navigator = navigator;
            this.domainFacade = domainFacade;
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
                .Button("Yes", new ExecutiveCommand(domainFacade, categoryKey))
                .ButtonClose("No")
                .Show();
        }

        private class ExecutiveCommand : Command
        {
            private readonly IDomainFacade domainFacade;
            private readonly IKey categoryKey;

            public ExecutiveCommand(IDomainFacade domainFacade, IKey categoryKey)
            {
                this.domainFacade = domainFacade;
                this.categoryKey = categoryKey;
            }

            public override bool CanExecute()
            {
                return true;
            }

            public override void Execute()
            {
                domainFacade.DeleteCategoryAsync(categoryKey);
            }
        }
    }
}
