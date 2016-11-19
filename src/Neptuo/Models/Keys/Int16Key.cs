using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Keys
{
    /// <summary>
    /// Implementation of <see cref="IKey"/> that takes single <see cref="short"/> value (ID).
    /// </summary>
    public class Int16Key : KeyBase
    {
        /// <summary>
        /// Creates new instance with typical <see cref="short"/> <paramref name="id"/>.
        /// </summary>
        /// <param name="id"><see cref="short"/> value of the key.</param>
        /// <param name="type">Type of the object.</param>
        /// <returns>Instance with typical <see cref="short"/> <paramref name="id"/>.</returns>
        public static Int16Key Create(short id, string type)
        {
            Ensure.Positive(id, "id");
            Ensure.NotNullOrEmpty(type, "type");
            return new Int16Key(id, type);
        }

        /// <summary>
        /// Creates new instance of empty key for <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <returns>Instance of empty key for <paramref name="type"/>.</returns>
        public static Int16Key Empty(string type)
        {
            Ensure.NotNullOrEmpty(type, "type");
            return new Int16Key(type);
        }

        /// <summary>
        /// Key value.
        /// </summary>
        public short ID { get; private set; }

        protected Int16Key(string type)
            : base(type, true)
        { }

        protected Int16Key(short id, string type)
            : base(type, false)
        {
            ID = id;
        }

        protected override bool Equals(KeyBase other)
        {
            Int16Key key;
            if (!TryConvert(other, out key))
                return false;

            return ID == key.ID;
        }

        protected override int CompareValueTo(KeyBase other)
        {
            Int16Key key;
            if (!TryConvert(other, out key))
                return 1;

            return ID.CompareTo(key.ID);
        }

        protected override int GetValueHashCode()
        {
            return ID.GetHashCode();
        }

        protected override string ToStringValue()
        {
            return ID.ToString();
        }
    }
}
