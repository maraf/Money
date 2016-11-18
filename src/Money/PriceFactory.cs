using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// A factory for price of single currency.
    /// </summary>
    public class PriceFactory : IFactory<Price, decimal>
    {
        public string Currency { get; private set; }

        public PriceFactory(string currency)
        {
            Currency = currency;
        }

        public Price Create(decimal value)
        {
            return new Price(value, Currency);
        }
    }
}
