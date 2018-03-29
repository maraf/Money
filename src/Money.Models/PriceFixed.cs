using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class PriceFixed : IPriceFixed
    {
        public Price Amount { get; }
        public DateTime When { get; }

        public PriceFixed(Price amount, DateTime when)
        {
            Ensure.NotNull(amount, "amount");
            Amount = amount;
            When = when;
        }
    }
}
