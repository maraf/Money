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
        private readonly CultureInfo cultureInfo;

        public DefaultCurrency(CultureInfo cultureInfo)
        {
            Ensure.NotNull(cultureInfo, "cultureInfo");
            this.cultureInfo = cultureInfo;
        }

        public ICurrency ForCustomSymbol(string symbol)
        {
            CultureInfo clone = (CultureInfo)cultureInfo.Clone();
            clone.NumberFormat.CurrencySymbol = symbol;
            return new DefaultCurrency(clone);
        }

        public string Format(decimal amount)
        {
            return amount.ToString("C", cultureInfo);
        }
    }
}
