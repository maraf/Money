using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    /// <summary>
    /// Provides ability to clone object or clone to instance of another type.
    /// </summary>
    /// <typeparam name="T">Target object type.</typeparam>
    public interface ICloneable<out T>
    {
        /// <summary>
        /// Creates new instance of type <typeparamref name="T"/> based on state of current object.
        /// </summary>
        /// <returns>New instance of type <typeparamref name="T"/> based on state current object.</returns>
        T Clone();
    }
}
