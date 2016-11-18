using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Observables.Commands
{
    public abstract class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        protected void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        public abstract bool CanExecute();

        void ICommand.Execute(object parameter)
        {
            Execute();
        }

        public abstract void Execute();
    }

    public abstract class Command<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;

        protected void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }

        public abstract bool CanExecute(T parameter);

        void ICommand.Execute(object parameter)
        {
            Execute((T)parameter);
        }

        public abstract void Execute(T parameter);
    }
}
