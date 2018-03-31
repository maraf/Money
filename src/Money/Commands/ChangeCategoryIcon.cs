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
    /// Changes a icon of a category with a key <see cref="CategoryKey"/>.
    /// </summary>
    public class ChangeCategoryIcon : Command
    {
        /// <summary>
        /// Gets a key of the category to modify.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Gets a new icon of the category.
        /// </summary>
        public string Icon { get; private set; }

        /// <summary>
        /// Changes a font icon for a category with a key <paramref name="categoryKey"/>.
        /// </summary>
        /// <param name="categoryKey">A key of the category.</param>
        /// <param name="icon">A font icon of the category</param>
        public ChangeCategoryIcon(IKey categoryKey, string icon)
        {
            Ensure.Condition.NotEmptyKey(categoryKey);
            CategoryKey = categoryKey;
            Icon = icon;
        }
    }
}
