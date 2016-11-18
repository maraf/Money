using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// A model for describing price.
    /// </summary>
    public class Price
    {
        /// <summary>
        /// Gets a value of the price.
        /// </summary>
        public decimal Value { get; private set; }

        /// <summary>
        /// Gets a currency of the price.
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="value">The value of the price.</param>
        /// <param name="currency">The currency of the price.</param>
        public Price(decimal value, string currency)
        {
            Ensure.NotNullOrEmpty(currency, "currency");
            Value = value;
            Currency = currency;
        }

        /// <summary>
        /// Creates a new zero price.
        /// </summary>
        /// <param name="currency">The currency of the price.</param>
        /// <returns>The new zero price.</returns>
        public static Price Zero(string currency)
        {
            return new Price(0, currency);
        }

        public override string ToString()
        {
            return $"{Value:N} {Currency}";
        }

        public static Price operator +(Price price1, Price price2)
        {
            Ensure.NotNull(price1, "price1");
            Ensure.NotNull(price2, "price2");

            if (price1.Currency != price2.Currency)
                throw Ensure.Exception.ArgumentOutOfRange("price1", "Currency doesn't match.");

            return new Price(price1.Value + price2.Value, price1.Currency);
        }

        public static Price operator -(Price price1, Price price2)
        {
            Ensure.NotNull(price1, "price1");
            Ensure.NotNull(price2, "price2");

            if (price1.Currency != price2.Currency)
                throw Ensure.Exception.ArgumentOutOfRange("price1", "Currency doesn't match.");

            return new Price(price1.Value - price2.Value, price1.Currency);
        }
    }
}
