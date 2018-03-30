using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Converters
{
    /// <summary>
    /// The static class with names used in serializing and deserializing keys
    /// </summary>
    public static class JsonName
    {
        /// <summary>
        /// The name of the item with the key type.
        /// </summary>
        public const string Type = "Type";

        /// <summary>
        /// The name of the <see cref="StringKey"/> value.
        /// </summary>
        public const string StringValue = "Identifier";

        /// <summary>
        /// The name of the <see cref="GuidKey"/> value.
        /// </summary>
        public const string GuidValue = "Guid";
    }
}
