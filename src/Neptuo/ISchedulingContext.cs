using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    /// <summary>
    /// The context information passed to the <see cref="ISchedulingProvider.Schedule(ISchedulingContext)"/> to be executed in the future.
    /// </summary>
    public interface ISchedulingContext
    {
        /// <summary>
        /// Gets the envelope to schedule for execution in the future.
        /// </summary>
        Envelope Envelope { get; }

        /// <summary>
        /// The method that should be executed when it's time.
        /// </summary>
        void Execute();
    }
}
