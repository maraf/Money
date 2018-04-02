using Neptuo.Collections.Specialized;
using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    public interface ICompositeFormatterExtender
    {
        /// <summary>
        /// Stores <paramref name="input"/> properties to the <paramref name="storage"/>.
        /// </summary>
        /// <param name="storage">The storage to save values to.</param>
        /// <param name="input">The event payload to store.</param>
        void Store(IKeyValueCollection storage, object input);

        /// <summary>
        /// Loads <paramref name="output"/> properties from the <paramref name="storage"/>.
        /// </summary>
        /// <param name="storage">The storage to load values from.</param>
        /// <param name="output">The event payload to load.</param>
        void Load(IReadOnlyKeyValueCollection storage, object output);
    }
}
