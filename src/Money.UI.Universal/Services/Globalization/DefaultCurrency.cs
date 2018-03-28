using Neptuo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services.Globalization
{
    public class DefaultCurrency : ICurrency
    {
        private readonly CultureInfo culture;
        private readonly RegionInfo region;

        public string UniqueCode => region.ISOCurrencySymbol;
        public string Symbol => culture.NumberFormat.CurrencySymbol;

        public DefaultCurrency(CultureInfo culture, RegionInfo region)
        {
            Ensure.NotNull(culture, "culture");
            Ensure.NotNull(region, "region");
            this.culture = culture;
            this.region = region;
        }

        public ICurrency ForCustomSymbol(string symbol)
        {
            CultureInfo clone = (CultureInfo)culture.Clone();
            clone.NumberFormat.CurrencySymbol = symbol;
            return new DefaultCurrency(clone, region);
        }

        public string Format(decimal amount)
        {
            return amount.ToString("C", culture);
        }
    }
}
