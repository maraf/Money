using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Parameters
{
    public class CategoryListParameter
    {
        public IKey Key { get; private set; }

        public CategoryListParameter(IKey key)
        {
            Ensure.Condition.NotEmptyKey(key);
            Key = key;
        }

        public CategoryListParameter()
        {
            Key = KeyFactory.Empty(typeof(Category));
        }
    }
}
