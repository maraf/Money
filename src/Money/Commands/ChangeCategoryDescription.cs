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
    /// Changes a description of a category with a key <see cref="CategoryKey"/>.
    /// </summary>
    public class ChangeCategoryDescription : Command
    {
        /// <summary>
        /// Gets a key of the category to modify.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Gets a new description of the category.
        /// </summary>
        public string Description { get; private set; }

        public ChangeCategoryDescription(IKey categoryKey, string description)
        {
            Ensure.Condition.NotEmptyKey(categoryKey);
            Ensure.NotNull(description, "description");
            CategoryKey = categoryKey;
            Description = description;
        }
    }
}
