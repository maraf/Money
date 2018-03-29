using Money.Commands;
using Neptuo;
using Neptuo.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Money.ViewModels.Commands
{
    public class CurrencySetAsDefaultCommand : DomainCommand<SetCurrencyAsDefault>
    {
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

        public CurrencySetAsDefaultCommand(ICommandDispatcher commandDispatcher, string name)
            : base(commandDispatcher)
        {
            Ensure.NotNull(name, "name");
            this.name = name;

            IsExecutable = true;
        }

        public override bool CanExecute() => IsExecutable;

        protected override SetCurrencyAsDefault CreateDomainCommand() => new SetCurrencyAsDefault(name);
    }
}
