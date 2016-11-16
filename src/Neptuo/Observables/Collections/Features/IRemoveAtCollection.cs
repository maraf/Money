using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Observables.Collections.Features
{
    /// <summary>
    /// Defines collection, that can remove items by index.
    /// </summary>
    public interface IRemoveAtCollection
    {
        /// <summary>
        /// Removes item at <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Index to remove item at.</param>
        void RemoveAt(int index);
    }
}
