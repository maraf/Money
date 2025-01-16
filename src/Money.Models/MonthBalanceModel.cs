using Neptuo;
using Neptuo.Formatters.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class MonthBalanceModel : MonthModel
    {
        [CompositeProperty(1, Version = 1)]
        [CompositeProperty(1, Version = 2)]
        public override int Year => base.Year;

        [CompositeProperty(2, Version = 1)]
        [CompositeProperty(2, Version = 2)]
        public override int Month => base.Month;

        [CompositeProperty(3, Version = 1)]
        [CompositeProperty(3, Version = 2)]
        public Price ExpenseSummary { get; private set; }

        [CompositeProperty(4, Version = 1)]
        [CompositeProperty(4, Version = 2)]
        public Price IncomeSummary { get; private set; }

        [CompositeProperty(5, Version = 2)]
        public Price ExpectedExpenseSummary { get; private set; }

        [CompositeVersion]
        public int Version { get; private set; }

        [CompositeConstructor(Version = 1)]
        public MonthBalanceModel(int year, int month, Price expenseSummary, Price incomeSummary)
            : base(year, month)
        {
            Ensure.NotNull(expenseSummary, "expenseSummary");
            Ensure.NotNull(incomeSummary, "incomeSummary");
            ExpenseSummary = expenseSummary;
            IncomeSummary = incomeSummary;
            Version = 1;
        }

        [CompositeConstructor(Version = 2)]
        public MonthBalanceModel(int year, int month, Price expenseSummary, Price incomeSummary, Price expectedExpenseSummary)
            : this(year, month, expenseSummary, incomeSummary)
        {
            Ensure.NotNull(expectedExpenseSummary, "expectedExpenseSummary");
            ExpectedExpenseSummary = expectedExpenseSummary;
            Version = 2;
        }
    }
}
