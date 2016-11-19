using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    /// <summary>
    /// Defines component with manual serialization and deserialization.
    /// </summary>
    public interface ICompositeModel
    {
        /// <summary>
        /// Saves current state to the <paramref name="storage"/>.
        /// </summary>
        /// <param name="storage">The storage to save values to.</param>
        void Save(ICompositeStorage storage);

        /// <summary>
        /// Loads state from <paramref name="storage"/>.
        /// </summary>
        /// <param name="storage">The storage to load values from.</param>
        void Load(ICompositeStorage storage);
    }
}
