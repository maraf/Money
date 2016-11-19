using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Keys
{
    /// <summary>
    /// Implementation of <see cref="IKey"/> that takes single <see cref="Guid"/> value.
    /// </summary>
    public class GuidKey : KeyBase
    {
        /// <summary>
        /// Creates new instance with typical <see cref="Guid"/> <paramref name="guid"/>.
        /// </summary>
        /// <param name="guid"><see cref="Guid"/> value of the key.</param>
        /// <param name="type">Type of the object.</param>
        /// <returns>Instance with typical <see cref="Guid"/> <paramref name="guid"/>.</returns>
        public static GuidKey Create(Guid guid, string type)
        {
            Ensure.NotNullOrEmpty(type, "type");
            return new GuidKey(guid, type);
        }

        /// <summary>
        /// Creates new instance of empty key for <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <returns>Instance of empty key for <paramref name="type"/>.</returns>
        public static GuidKey Empty(string type)
        {
            Ensure.NotNullOrEmpty(type, "type");
            return new GuidKey(type);
        }

        public Guid Guid { get; private set; }
        
        protected GuidKey(string type)
            : base(type, true)
        { }

        protected GuidKey(Guid guid, string type)
            : base(type, false)
        {
            Guid = guid;
        }

        protected override bool Equals(KeyBase other)
        {
            GuidKey key;
            if (!TryConvert(other, out key))
                return false;

            return Guid == key.Guid;
        }

        protected override int CompareValueTo(KeyBase other)
        {
            GuidKey key;
            if (!TryConvert(other, out key))
                return 1;

            return Guid.CompareTo(key.Guid);
        }

        protected override int GetValueHashCode()
        {
            return Guid.GetHashCode();
        }

        protected override string ToStringValue()
        {
            return Guid.ToString();
        }
    }
}
