using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// A model of a single user property.
    /// </summary>
    public class UserPropertyModel : ICloneable<UserPropertyModel>
    {
        /// <summary>
        /// Gets a property key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Get a property value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Creates a new empty instance.
        /// </summary>
        public UserPropertyModel()
        { }

        /// <summary>
        /// Creates a new instance with <paramref name="key"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="key">A property key.</param>
        /// <param name="value">A property value.</param>
        public UserPropertyModel(string key, string value)
        {
            Ensure.NotNull(key, "key");
            Key = key;
            Value = value;
        }

        public UserPropertyModel Clone() 
            => new UserPropertyModel(Key, Value);
    }
}
