using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Keys
{
    /// <summary>
    /// Implementation of <see cref="IKey"/> that takes single <see cref="long"/> value (ID).
    /// </summary>
    public class Int64Key : KeyBase
    {
        /// <summary>
        /// Creates new instance with typical <see cref="long"/> <paramref name="id"/>.
        /// </summary>
        /// <param name="id"><see cref="long"/> value of the key.</param>
        /// <param name="type">Type of the object.</param>
        /// <returns>Instance with typical <see cref="long"/> <paramref name="id"/>.</returns>
        public static Int64Key Create(long id, string type)
        {
            Ensure.Positive(id, "id");
            Ensure.NotNullOrEmpty(type, "type");
            return new Int64Key(id, type);
        }

        /// <summary>
        /// Creates new instance of empty key for <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <returns>Instance of empty key for <paramref name="type"/>.</returns>
        public static Int64Key Empty(string type)
        {
            Ensure.NotNullOrEmpty(type, "type");
            return new Int64Key(type);
        }

        /// <summary>
        /// Key value.
        /// </summary>
        public long ID { get; private set; }

        protected Int64Key(string type)
            : base(type, true)
        { }

        protected Int64Key(long id, string type)
            : base(type, false)
        {
            ID = id;
        }

        protected override bool Equals(KeyBase other)
        {
            Int64Key key;
            if (!TryConvert(other, out key))
                return false;

            return ID == key.ID;
        }

        protected override int CompareValueTo(KeyBase other)
        {
            Int64Key key;
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
