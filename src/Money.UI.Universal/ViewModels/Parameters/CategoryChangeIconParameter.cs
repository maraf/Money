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
    /// A parameter for navigation to change category icon.
    /// </summary>
    public class CategoryChangeIconParameter
    {
        public IKey CategoryKey { get; private set; }

        public CategoryChangeIconParameter(IKey categoryKey)
        {
            Ensure.Condition.NotEmptyKey(categoryKey);
            CategoryKey = categoryKey;
        }
    }
}
