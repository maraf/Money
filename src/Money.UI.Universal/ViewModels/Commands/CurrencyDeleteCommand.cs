using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Money.ViewModels.Navigation;
using Neptuo;
using Money.Services;

namespace Money.ViewModels.Commands
{
    /// <summary>
    /// A command for deleting currency with confirmation.
    /// </summary>
    public class CurrencyDeleteCommand : Command
    {
        private readonly INavigator navigator;
        private readonly IDomainFacade domainFacade;
        private readonly MessageBuilder messageBuilder;
        private readonly string uniqueCode;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="navigator">A navigator for creating confirmation prompt.</param>
        /// <param name="domainFacade">A domain facade.</param>
        /// <param name="messageBuilder">User message builder.</param>
        /// <param name="uniqueCode">An unique currency code to delete.</param>
        public CurrencyDeleteCommand(INavigator navigator, IDomainFacade domainFacade, MessageBuilder messageBuilder, string uniqueCode)
        {
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(domainFacade, "domainFacade");
            Ensure.NotNull(messageBuilder, "messageBuilder");
            Ensure.NotNullOrEmpty(uniqueCode, "uniqueCode");
            this.navigator = navigator;
            this.domainFacade = domainFacade;
            this.messageBuilder = messageBuilder;
            this.uniqueCode = uniqueCode;
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override void Execute()
        {
            navigator.Message($"Do you really want to delete currency '{uniqueCode}'? {Environment.NewLine} {Environment.NewLine}You won't be able to restore it later or create a currency with the same unique code.")
                .Button("Yes", new YesCommand(navigator, domainFacade, messageBuilder, uniqueCode))
                .ButtonClose("No")
                .Show();
        }

        private class YesCommand : Command
        {
            private readonly INavigator navigator;
            private readonly IDomainFacade domainFacade;
            private readonly MessageBuilder messageBuilder;
            private readonly string uniqueCode;

            public YesCommand(INavigator navigator, IDomainFacade domainFacade, MessageBuilder messageBuilder, string uniqueCode)
            {
                this.navigator = navigator;
                this.domainFacade = domainFacade;
                this.messageBuilder = messageBuilder;
                this.uniqueCode = uniqueCode;
            }

            public override bool CanExecute()
            {
                return true;
            }

            public async override void Execute()
            {
                try
                {
                    await domainFacade.DeleteCurrencyAsync(uniqueCode);
                }
                catch(CantDeleteDefaultCurrencyException)
                {
                    navigator
                        .Message(messageBuilder.CantDeleteDefaultCurrency())
                        .Show();
                }
                catch(CantDeleteLastCurrencyException)
                {
                    navigator
                        .Message(messageBuilder.CantDeleteLastCurrency())
                        .Show();
                }
            }
        }
    }
}
