using Neptuo.Collections.Specialized;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    /// <summary>
    /// Stores/Loads all-commands-shared properties with internal setters.
    /// </summary>
    public class CommandExtender : ICompositeFormatterExtender
    {
        /// <summary>
        /// The names of the keys used in store/load methods.
        /// </summary>
        protected static class Name
        {
            /// <summary>
            /// The name where the key of the command is stored.
            /// </summary>
            public const string Key = "Key";
        }

        /// <summary>
        /// Stores <paramref name="payload"/> properties to the <paramref name="storage"/>.
        /// </summary>
        /// <param name="storage">The storage to save values to.</param>
        /// <param name="payload">The command payload to store.</param>
        public void Store(IKeyValueCollection storage, object input)
        {
            Command payload = input as Command;
            if (payload != null)
                storage.Add(Name.Key, payload.Key);
        }

        /// <summary>
        /// Loads <paramref name="payload"/> properties from the <paramref name="storage"/>.
        /// </summary>
        /// <param name="storage">The storage to load values from.</param>
        /// <param name="payload">The command payload to load.</param>
        public void Load(IReadOnlyKeyValueCollection storage, object output)
        {
            Command payload = output as Command;
            if (payload != null)
                payload.Key = storage.Get<IKey>(Name.Key);
        }
    }
}
