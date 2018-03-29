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
    /// Renames a category with a key <see cref="CategoryKey"/>.
    /// </summary>
    public class RenameCategory : Command
    {
        /// <summary>
        /// Gets a key of the category to rename.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Gets a new name of the category.
        /// </summary>
        public string NewName { get; private set; }

        /// <summary>
        /// Renames a category with a key <paramref name="categoryKey"/>.
        /// </summary>
        /// <param name="categoryKey">A key of the category to rename.</param>
        /// <param name="newName">A new name of the category.</param>
        public RenameCategory(IKey categoryKey, string newName)
        {
            Ensure.Condition.NotEmptyKey(categoryKey);
            Ensure.NotNull(newName, "newName");
            CategoryKey = categoryKey;
            NewName = newName;
        }
    }
}
