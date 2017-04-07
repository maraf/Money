using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Observables.Commands
{
    /// <summary>
    /// A command that cancels a command implementing <see cref="ICancellableCommand"/>.
    /// </summary>
    public class CancelCommand : Command
    {
        private readonly ICancellableCommand command;

        /// <summary>
        /// Creates a new instance that cancels <paramref name="command"/>.
        /// </summary>
        /// <param name="command">A command to cancel.</param>
        public CancelCommand(ICancellableCommand command)
        {
            Ensure.NotNull(command, "command");
            this.command = command;
            this.command.CanExecuteChanged += OnCanExecuteChanged;
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        public override bool CanExecute()
        {
            return command.IsRunning;
        }

        public override void Execute()
        {
            if (CanExecute())
                command.Cancel();
        }
    }
}
