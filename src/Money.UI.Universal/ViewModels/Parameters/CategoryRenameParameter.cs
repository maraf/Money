using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Parameters
{
    /// <summary>
    /// A parameter for renaming a category.
    /// </summary>
    public class CategoryRenameParameter
    {
        /// <summary>
        /// Gets a key for the category to rename.
        /// </summary>
        public IKey Key { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="key">A key for the category to rename.</param>
        public CategoryRenameParameter(IKey key)
        {
            Ensure.Condition.NotEmptyKey(key);
            Key = key;
        }
    }
}
