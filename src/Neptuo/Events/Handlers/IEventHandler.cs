using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Events.Handlers
{
    /// <summary>
    /// Handler for events of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IEventHandler<TEvent>
    {
        /// <summary>
        /// Handles event described by <paramref name="payload"/>.
        /// </summary>
        /// <param name="payload">Instance of event data.</param>
        Task HandleAsync(TEvent payload);
    }
}
