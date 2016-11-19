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
    /// Describes contract for getting and storing domain models by its key.
    /// </summary>
    /// <typeparam name="TDomainModel">The type of the domain model.</typeparam>
    /// <typeparam name="TKey">The type of the domain model key.</typeparam>
    public interface IRepository<TDomainModel, in TKey> : IReadOnlyRepository<TDomainModel, TKey>
        where TKey : IKey
        where TDomainModel : IDomainModel<TKey>
    {
        /// <summary>
        /// Saves changes to the <paramref name="model"/> to the underlaying storage.
        /// </summary>
        /// <param name="model">The instance of the model to save.</param>
        void Save(TDomainModel model);
    }
}
