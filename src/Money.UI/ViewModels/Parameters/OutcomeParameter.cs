using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Parameters
{
    public class OutcomeParameter
    {
        public Price Amount { get; set; }
        public string Description { get; set; }

        public IKey CategoryKey { get; private set; }

        public OutcomeParameter()
        {
            CategoryKey = KeyFactory.Empty(typeof(Category));
        }

        public OutcomeParameter(IKey categoryKey)
        {
            Ensure.Condition.NotEmptyKey(categoryKey);
            CategoryKey = categoryKey;
        }
    }
}
