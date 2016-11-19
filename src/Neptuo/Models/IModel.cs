using Neptuo.Models.Domains;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models
{
    /// <summary>
    /// Describes model with key.
    /// This key can be read and set.
    /// </summary>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    public interface IModel<TKey> : IDomainModel<TKey>
        where TKey : IKey
    {
        /// <summary>
        /// Model key.
        /// Should never be <c>null</c>. Instead use empty key.
        /// </summary>
        new TKey Key { get; set; }
    }
}
