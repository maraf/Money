using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Keys
{
    /// <summary>
    /// Implementation of <see cref="IKey"/> that takes single <see cref="int"/> value (ID).
    /// </summary>
    public class Int32Key : KeyBase
    {
        /// <summary>
        /// Creates new instance with typical <see cref="int"/> <paramref name="id"/>.
        /// </summary>
        /// <param name="id"><see cref="int"/> value of the key.</param>
        /// <param name="type">Type of the object.</param>
        /// <returns>Instance with typical <see cref="int"/> <paramref name="id"/>.</returns>
        public static Int32Key Create(int id, string type)
        {
            Ensure.Positive(id, "id");
            Ensure.NotNullOrEmpty(type, "type");
            return new Int32Key(id, type);
        }

        /// <summary>
        /// Creates new instance of empty key for <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <returns>Instance of empty key for <paramref name="type"/>.</returns>
        public static Int32Key Empty(string type)
        {
            Ensure.NotNullOrEmpty(type, "type");
            return new Int32Key(type);
        }

        /// <summary>
        /// Key value.
        /// </summary>
        public int ID { get; private set; }

        protected Int32Key(string type)
            : base(type, true)
        { }

        protected Int32Key(int id, string type)
            : base(type, false)
        {
            ID = id;
        }

        protected override bool Equals(KeyBase other)
        {
            Int32Key key;
            if (!TryConvert(other, out key))
                return false;

            return ID == key.ID;
        }

        protected override int CompareValueTo(KeyBase other)
        {
            Int32Key key;
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
