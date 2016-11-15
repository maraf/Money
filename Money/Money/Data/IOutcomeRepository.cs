using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Data
{
    /// <summary>
    /// Describes contract for loading and saving outcome models.
    /// </summary>
    public interface IOutcomeRepository
    {
        /// <summary>
        /// Enumerates all outcomes.
        /// </summary>
        /// <returns>Enumeration of all outcomes.</returns>
        IEnumerable<Outcome> Enumerate();

        /// <summary>
        /// Saves <paramref name="model "/>.
        /// </summary>
        /// <param name="model">The outcome to save (insert or update).</param>
        void Save(Outcome model);

        /// <summary>
        /// Deletes outcome by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of outcome to delete.</param>
        void Delete(Guid id);
    }
}
