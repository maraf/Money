using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Keys
{
    /// <summary>
    /// Describes key of the model.
    /// </summary>
    public interface IKey : IEquatable<IKey>, IComparable
    {
        /// <summary>
        /// Identifier of the model type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Whether this key is empty.
        /// </summary>
        bool IsEmpty { get; }
    }
}
