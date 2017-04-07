using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Money.ViewModels.Commands
{
    public class CurrencySetAsDefaultCommand : AsyncCommand
    {
        private readonly IDomainFacade domainFacade;
        private readonly string name;

        public CurrencySetAsDefaultCommand(IDomainFacade domainFacade, string name)
        {
            Ensure.NotNull(domainFacade, "domainFacade");
            Ensure.NotNull(name, "name");
            this.domainFacade = domainFacade;
            this.name = name;
        }

        protected override bool CanExecuteOverride()
        {
            return true;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return domainFacade.SetCurrencyAsDefaultAsync(name);
        }
    }
}
