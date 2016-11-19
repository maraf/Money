using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands
{
    /// <summary>
    /// Describes event-sourcing compatible command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// The key of this command.
        /// </summary>
        IKey Key { get; }
    }
}
