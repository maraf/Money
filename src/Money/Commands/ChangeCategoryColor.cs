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
    /// Changes a color of a category with a key <see cref="CategoryKey"/>.
    /// </summary>
    public class ChangeCategoryColor : Command
    {
        /// <summary>
        /// Gets a key of the category to modify.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Gets a new color of the category.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Changes a color of a category with a key <paramref name="categoryKey"/>.
        /// </summary>
        /// <param name="categoryKey">A key of the category.</param>
        /// <param name="color">A new color of the category.</param>
        public ChangeCategoryColor(IKey categoryKey, Color color)
        {
            Ensure.Condition.NotEmptyKey(categoryKey);
            CategoryKey = categoryKey;
            Color = color;
        }
    }
}
