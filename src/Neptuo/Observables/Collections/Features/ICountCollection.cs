using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Observables.Collections.Features
{
    /// <summary>
    /// Defines collection, that is counted.
    /// </summary>
    public interface ICountCollection
    {
        /// <summary>
        /// Count of items in collection.
        /// </summary>
        int Count { get; }
    }
}
