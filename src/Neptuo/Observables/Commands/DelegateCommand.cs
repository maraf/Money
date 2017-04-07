using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Observables.Commands
{
    /// <summary>
    /// An implementation of <see cref="System.Windows.Input.ICommand"/> that takes delegates for 'execute' and 'can execute' methods.
    /// </summary>
    public class DelegateCommand : Command
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        /// <summary>
        /// Creates new instance where <paramref name="execute"/> and <paramref name="canExecute"/> can't be <c>null</c>.
        /// </summary>
        /// <param name="execute">A delegate to be executed when the command is executed.</param>
        /// <param name="canExecute">A delegate to be execute when the 'can execute' is executed.</param>
        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            Ensure.NotNull(execute, "execute");
            Ensure.NotNull(canExecute, "canExecute");
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Creates new instance where 'can execute' returns always <c>true</c> and <paramref name="execute"/> can't be <c>null</c>.
        /// </summary>
        /// <param name="execute">A delegate to be executed when the command is executed.</param>
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

    /// <summary>
    /// An implementation of <see cref="System.Windows.Input.ICommand"/> with parameter of type <typeparam name="T" /> 
    /// that takes delegates for 'execute' and 'can execute' methods.
    /// </summary>
    /// <typeparam name="T">A type of the parameter.</typeparam>
    public class DelegateCommand<T> : Command<T>
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool> canExecute;

        /// <summary>
        /// Creates new instance where <paramref name="execute"/> and <paramref name="canExecute"/> can't be <c>null</c>.
        /// </summary>
        /// <param name="execute">A delegate to be executed when the command is executed.</param>
        /// <param name="canExecute">A delegate to be execute when the 'can execute' is executed.</param>
        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            Ensure.NotNull(execute, "execute");
            Ensure.NotNull(canExecute, "canExecute");
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Creates new instance where 'can execute' returns always <c>true</c> and <paramref name="execute"/> can't be <c>null</c>.
        /// </summary>
        /// <param name="execute">A delegate to be executed when the command is executed.</param>
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