using Money.Models.Queries;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Observables;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    /// <summary>
    /// A view model for sum of summary items.
    /// </summary>
    public class SummaryTotalViewModel : ObservableObject, ISummaryItemViewModel
    {
        public IKey CategoryKey { get; private set; }
        public Price Amount { get; private set; }

        public SummaryTotalViewModel(Price value)
        {
            Ensure.NotNull(value, "value");
            CategoryKey = KeyFactory.Empty(typeof(Category));
            Amount = value;
        }
    }
}
