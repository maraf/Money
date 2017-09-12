using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Money.ViewModels.Navigation;
using Neptuo;

namespace Money.ViewModels.Commands
{
    /// <summary>
    /// A command for deleting currency with confirmation.
    /// </summary>
    public class CurrencyDeleteCommand : Command
    {
        private readonly INavigator navigator;
        private readonly IDomainFacade domainFacade;
        private readonly string uniqueCode;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="navigator">A navigator for creating confirmation prompt.</param>
        /// <param name="domainFacade">A domain facade.</param>
        /// <param name="uniqueCode">An unique currency code to delete.</param>
        public CurrencyDeleteCommand(INavigator navigator, IDomainFacade domainFacade, string uniqueCode)
        {
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(domainFacade, "domainFacade");
            Ensure.NotNullOrEmpty(uniqueCode, "uniqueCode");
            this.navigator = navigator;
            this.domainFacade = domainFacade;
            this.uniqueCode = uniqueCode;
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override void Execute()
        {
            navigator.Message($"Do you really want to delete currency '{uniqueCode}'? {Environment.NewLine} {Environment.NewLine}You won't be able to restore it later or create a currency with the same unique code.")
                .Button("Yes", new YesCommand(domainFacade, uniqueCode))
                .ButtonClose("No")
                .Show();
        }

        private class YesCommand : Command
        {
            private readonly IDomainFacade domainFacade;
            private readonly string uniqueCode;

            public YesCommand(IDomainFacade domainFacade, string uniqueCode)
            {
                this.domainFacade = domainFacade;
                this.uniqueCode = uniqueCode;
            }

            public override bool CanExecute()
            {
                return true;
            }

            public override void Execute()
            {
                domainFacade.DeleteCurrencyAsync(uniqueCode);
            }
        }
    }
}
