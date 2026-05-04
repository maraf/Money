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
        private readonly int decimalDigits;
        private Dictionary<string, CultureInfo> currencies;

        public CurrencyFormatter(List<CurrencyModel> models, int decimalDigits)
        {
            Ensure.NotNull(models, "models");
            Ensure.PositiveOrZero(decimalDigits, "decimalDigits");
            this.models = models;
            this.decimalDigits = decimalDigits;
        }

        private void EnsureCurrencies()
        {
            if (currencies == null)
            {
                IEnumerable<CultureInfo> cultures = CultureInfo
                    .GetCultures(CultureTypes.AllCultures)
                    .Where(c => !c.IsNeutralCulture);

                currencies = new Dictionary<string, CultureInfo>();
                foreach (CultureInfo culture in cultures)
                {
                    try
                    {
                        RegionInfo region = new RegionInfo(culture.Name);
                        currencies[region.ISOCurrencySymbol] = culture;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        public string Format(Price price, FormatZero zero = FormatZero.Zero, bool applyUserDigits = true, bool applyPlusForPositiveNumbers = false)
        {
            if (price == null)
            {
                return zero switch
                {
                    FormatZero.Empty => String.Empty,
                    FormatZero.Placehoder => "---",
                    _ => "---"
                };
            }

            if (price.Value == 0)
            {
                return zero switch
                {
                    FormatZero.Empty => String.Empty,
                    FormatZero.Placehoder => FormatPlaceholderWithCurrency(price),
                    _ => FormatInternal(price)
                };
            }

            return FormatInternal(price, applyUserDigits, applyPlusForPositiveNumbers);
        }

        private string FormatPlaceholderWithCurrency(Price price)
        {
            CurrencyModel currency = models.FirstOrDefault(c => c.UniqueCode == price.Currency);
            if (currency == null)
                return "---";

            return $"--- {currency.Symbol}";
        }

        private string FormatInternal(Price price, bool applyUserDigits = true, bool applyPlusForPositiveNumbers = false) 
        {
            Ensure.NotNull(price, "price");
            
            CurrencyModel currency = models.FirstOrDefault(c => c.UniqueCode == price.Currency);
            if (currency == null)
                return price.ToString();

            EnsureCurrencies();
            if (!currencies.TryGetValue(price.Currency, out var culture))
                culture = CultureInfo.CurrentCulture;

            int effectiveDigits = applyUserDigits ? GetEffectiveDecimalDigits(price.Value) : culture.NumberFormat.CurrencyDecimalDigits;

            var modifiedCulture = (CultureInfo)culture.Clone();
            modifiedCulture.NumberFormat.CurrencyDecimalDigits = effectiveDigits;
            modifiedCulture.NumberFormat.CurrencySymbol = currency.Symbol;

            string value = price.Value.ToString("C", modifiedCulture);
            if (applyPlusForPositiveNumbers && price.Value > 0)
                value = $"+{value}";

            return value;
        }

        private int GetEffectiveDecimalDigits(decimal value)
        {
            if (value != Math.Truncate(value))
                return Math.Max(decimalDigits, 2);

            return decimalDigits;
        }

        public enum FormatZero
        {
            Zero,
            Empty,
            Placehoder
        }
    }
}
