using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Observables.Commands
{
    public class DelegateCommand : Command
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            Ensure.NotNull(execute, "execute");
            Ensure.NotNull(canExecute, "canExecute");
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public DelegateCommand(Action execute)
            : this(execute, () => true)
        { }

        public override bool CanExecute()
        {
            return canExecute();
        }

        public override void Execute()
        {
            execute();
        }
    }

    public class DelegateCommand<T> : Command<T>
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool> canExecute;

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            Ensure.NotNull(execute, "execute");
            Ensure.NotNull(canExecute, "canExecute");
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public DelegateCommand(Action<T> execute)
            : this(execute, parameter => true)
        { }

        public override bool CanExecute(T parameter)
        {
            return canExecute(parameter);
        }

        public override void Execute(T parameter)
        {
            execute(parameter);
        }
    }
}
