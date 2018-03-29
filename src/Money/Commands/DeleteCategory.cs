using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    /// <summary>
    /// Deletes (soft) category with <see cref="CategoryKey"/>.
    /// </summary>
    public class DeleteCategory : Command
    {
        /// <summary>
        /// Gets a key of the category to delete.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Deletes (soft) category with <paramref name="categoryKey"/>.
        /// </summary>
        /// <param name="categoryKey">A key of the category.</param>
        public DeleteCategory(IKey categoryKey)
        {
            Ensure.Condition.NotEmptyKey(categoryKey);
            CategoryKey = categoryKey;
        }
    }
}
