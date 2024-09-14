using Neptuo;
using Neptuo.Formatters.Metadata;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    /// <summary>
    /// A query for getting list of categories with month outcome summary.
    /// </summary>
    public class ListMonthCategoryWithOutcome : UserQuery, IQuery<List<CategoryWithAmountModel>>
    {
        [CompositeProperty(0, Version = 1)]
        [CompositeProperty(0, Version = 2)]
        public MonthModel Month { get; private set; }

        [CompositeVersion]
        [CompositeProperty(1, Version = 2)]
        public int Version { get; private set; }

        [CompositeConstructor(Version = 1)]
        public ListMonthCategoryWithOutcome(MonthModel month)
            : this(month, 1)
        { }

        [CompositeConstructor(Version = 2)]
        public ListMonthCategoryWithOutcome(MonthModel month, int version = 2)
        {
            Ensure.NotNull(month, "month");
            Month = month;
            Version = version;
        }

        public static ListMonthCategoryWithOutcome Version2(MonthModel month)
            => new ListMonthCategoryWithOutcome(month, 2);
    }
}
