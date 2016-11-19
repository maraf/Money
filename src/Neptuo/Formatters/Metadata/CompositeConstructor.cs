using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    /// <summary>
    /// A description of version constructor.
    /// </summary>
    public class CompositeConstructor
    {
        /// <summary>
        /// The factory to create an instance from an array of parameters.
        /// </summary>
        public Func<object[], object> Factory { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="factory">The factory to create an instance from an array of parameters.</param>
        public CompositeConstructor(Func<object[], object> factory)
        {
            Ensure.NotNull(factory, "factory");
            Factory = factory;
        }
    }
}
