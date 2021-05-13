using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class YearWithAmountModel : YearModel
    {
        public override int Year => base.Year;

        public Price TotalAmount { get; private set; }

        public YearWithAmountModel(int year, Price totalAmount) : base(year)
        {
            TotalAmount = totalAmount;
        }
    }
}
