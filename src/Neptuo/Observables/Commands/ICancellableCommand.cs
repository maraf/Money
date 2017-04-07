using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Observables.Commands
{
    /// <summary>
    /// A command which runs asynchronously and can be cancelled.
    /// </summary>
    public interface ICancellableCommand : ICommand
    {
        /// <summary>
        /// Returns <c>true</c> is the command currently being executed; <c>false</c> otherwise.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Stops execution of the command.
        /// If not running, nothing should happen.
        /// </summary>
        void Cancel();
    }
}
