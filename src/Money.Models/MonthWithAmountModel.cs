using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class MonthWithAmountModel : MonthModel
    {
        public override int Year => base.Year;
        public override int Month => base.Month;

        public Price TotalAmount { get; private set; }

        public MonthWithAmountModel(int year, int month, Price totalAmount) : base(year, month)
        {
            TotalAmount = totalAmount;
        }
    }
}
