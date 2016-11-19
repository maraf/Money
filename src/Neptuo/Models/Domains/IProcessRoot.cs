using Neptuo.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Domains
{
    /// <summary>
    /// Describes event-sourcing compatible (long running) process.
    /// </summary>
    public interface IProcessRoot : IAggregateRoot
    {
        /// <summary>
        /// The enumeration of unpublished commands.
        /// </summary>
        IEnumerable<Envelope<ICommand>> Commands { get; }
    }
}
