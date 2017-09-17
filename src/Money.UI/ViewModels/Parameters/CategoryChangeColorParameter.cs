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
    /// A parameter for changing category color.
    /// </summary>
    public class CategoryChangeColorParameter
    {
        /// <summary>
        /// Gets a category key.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Cretas a new instance.
        /// </summary>
        /// <param name="categoryKey">A category key.</param>
        public CategoryChangeColorParameter(IKey categoryKey)
        {
            Ensure.Condition.NotEmptyKey(categoryKey);
            CategoryKey = categoryKey;
        }
    }
}
