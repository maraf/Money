using Neptuo;
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
        public override int Year => base.Year;
        public override int Month => base.Month;

        public Price ExpenseSummary { get; private set; }
        public Price IncomeSummary { get; private set; }

        public MonthBalanceModel(int year, int month, Price expenseSummary, Price incomeSummary)
            : base(year, month)
        {
            Ensure.NotNull(expenseSummary, "expenseSummary");
            Ensure.NotNull(incomeSummary, "incomeSummary");
            ExpenseSummary = expenseSummary;
            IncomeSummary = incomeSummary;
        }
    }
}
