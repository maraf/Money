using Neptuo.Models.Domains;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Repositories
{
    /// <summary>
    /// Describes contract for getting domain models by its key.
    /// </summary>
    public interface IReadOnlyRepository<TDomainModel, in TKey>
        where TKey : IKey
        where TDomainModel : IDomainModel<TKey>
    {
        /// <summary>
        /// Tries to find model with the key <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the model to find.</param>
        /// <returns>The instance of the model with the key; <c>null</c> if such model doesn't exist.</returns>
        TDomainModel Find(TKey key);
    }
}
