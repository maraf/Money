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

        private bool isExecutable;

        public bool IsExecutable
        {
            get { return isExecutable; }
            set
            {
                if (isExecutable != value)
                {
                    isExecutable = value;
                    RaiseCanExecuteChanged();
                }
            }
        }

        public CurrencySetAsDefaultCommand(IDomainFacade domainFacade, string name)
        {
            Ensure.NotNull(domainFacade, "domainFacade");
            Ensure.NotNull(name, "name");
            this.domainFacade = domainFacade;
            this.name = name;

            IsExecutable = true;
        }

        protected override bool CanExecuteOverride()
        {
            return IsExecutable;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return domainFacade.SetCurrencyAsDefaultAsync(name);
        }
    }
}
