using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Keys
{
    /// <summary>
    /// Implementation of <see cref="IKey"/> that takes single <see cref="string"/> identifier.
    /// </summary>
    public class StringKey : KeyBase
    {
        /// <summary>
        /// Creates new instance with <see cref="string"/> <paramref name="identifier" />.
        /// </summary>
        /// <param name="identifier">String identifier.</param>
        /// <param name="type">Type of the object.</param>
        /// <returns>Instance with <see cref="string"/> <paramref name="identifier" />.</returns>
        public static StringKey Create(string identifier, string type)
        {
            Ensure.NotNullOrEmpty(identifier, "identifier");
            Ensure.NotNullOrEmpty(type, "type");
            return new StringKey(identifier, type);
        }

        /// <summary>
        /// Creates new instance of empty key for <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <returns>Instance of empty key for <paramref name="type"/>.</returns>
        public static StringKey Empty(string type)
        {
            Ensure.NotNullOrEmpty(type, "type");
            return new StringKey(type);
        }

        /// <summary>
        /// Key value.
        /// </summary>
        public string Identifier { get; private set; }

        protected StringKey(string type)
            : base(type, true)
        { }

        protected StringKey(string identifier, string type)
            : base(type, false)
        {
            Identifier = identifier;
        }

        protected override bool Equals(KeyBase other)
        {
            StringKey key;
            if (!TryConvert(other, out key))
                return false;

            return Identifier == key.Identifier;
        }

        protected override int CompareValueTo(KeyBase other)
        {
            StringKey key;
            if (!TryConvert(other, out key))
                return 1;

            return Identifier.CompareTo(key.Identifier);
        }

        protected override int GetValueHashCode()
        {
            return Identifier.GetHashCode();
        }

        protected override string ToStringValue()
        {
            return Identifier;
        }
    }
}
