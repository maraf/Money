using Money.Models;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class CurrencyFormatter
    {
        private readonly List<CurrencyModel> models;
        private Dictionary<string, (CultureInfo culture, RegionInfo region)> currencies;

        public CurrencyFormatter(List<CurrencyModel> models)
        {
            Ensure.NotNull(models, "models");
            this.models = models;
        }

        private void EnsureCurrencies()
        {
            if (currencies == null)
            {
                IEnumerable<CultureInfo> cultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
                     .Where(c => !c.IsNeutralCulture);

                currencies = new Dictionary<string, (CultureInfo culture, RegionInfo region)>();
                foreach (CultureInfo culture in cultures)
                {
                    try
                    {
                        RegionInfo region = new RegionInfo(culture.Name);
                        currencies[region.ISOCurrencySymbol] = (culture, region);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        public string Format(Price price)
        {
            CurrencyModel currency = models.FirstOrDefault(c => c.UniqueCode == price.Currency);
            if (currency == null)
                return price.ToString();

            EnsureCurrencies();
            if (!currencies.TryGetValue(price.Currency, out var cultureInfo))
                cultureInfo = (CultureInfo.CurrentCulture, new RegionInfo(CultureInfo.CurrentCulture.Name));

            CultureInfo culture = cultureInfo.culture;
            culture = (CultureInfo)culture.Clone();
            culture.NumberFormat.CurrencySymbol = currency.Symbol;

            return price.Value.ToString("C", culture);
        }
    }
}
