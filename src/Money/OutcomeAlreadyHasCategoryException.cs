using Neptuo.Models;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// An exception raised when assigning category that already is assigned in the outcome.
    /// </summary>
    public class OutcomeAlreadyHasCategoryException : AggregateRootException
    {
        public IKey CategoryKey { get; private set; }

        public OutcomeAlreadyHasCategoryException(IKey categoryKey)
            : base(string.Format("The outcome already has a category '{0}'.", categoryKey))
        {
            CategoryKey = categoryKey;
        }
    }
}
