using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    internal static class Validator
    {
        #region Outcome

        public static bool IsOutcomeAmount(decimal amount) => amount > 0;
        public static bool IsOutcomeDescription(string description) => !String.IsNullOrEmpty(description);
        public static bool IsOutcomeCurrency(string currency) => !String.IsNullOrEmpty(currency);
        public static bool IsOutcomeCategoryKey(IKey categoryKey) => categoryKey != null && !categoryKey.IsEmpty;

        public static void AddOutcomeAmount(ICollection<string> messages, decimal amount)
        {
            if (!IsOutcomeAmount(amount))
                messages.Add("Amount must be greater than zero.");
        }

        public static void AddOutcomeDescription(ICollection<string> messages, string description)
        {
            if (!IsOutcomeDescription(description))
                messages.Add("Description must be provided.");
        }

        public static void AddOutcomeCurrency(ICollection<string> messages, string currency)
        {
            if (!IsOutcomeCurrency(currency))
                messages.Add("Currency must be selected.");
        }

        public static void AddOutcomeCategoryKey(ICollection<string> messages, IKey categoryKey)
        {
            if (!IsOutcomeCategoryKey(categoryKey))
                messages.Add("Category must be selected.");
        }

        #endregion

        #region Income

        public static bool IsIncomeAmount(decimal amount) => amount > 0;
        public static bool IsIncomeDescription(string description) => !String.IsNullOrEmpty(description);
        public static bool IsIncomeCurrency(string currency) => !String.IsNullOrEmpty(currency);

        public static void AddIncomeAmount(ICollection<string> messages, decimal amount)
        {
            if (!IsIncomeAmount(amount))
                messages.Add("Amount must be greater than zero.");
        }

        public static void AddIncomeDescription(ICollection<string> messages, string description)
        {
            if (!IsIncomeDescription(description))
                messages.Add("Description must be provided.");
        }

        public static void AddIncomeCurrency(ICollection<string> messages, string currency)
        {
            if (!IsIncomeCurrency(currency))
                messages.Add("Currency must be selected.");
        }

        #endregion

        #region Category

        public static bool IsCategoryName(string name) => !String.IsNullOrEmpty(name);
        public static bool IsCategoryDescription(string description) => !String.IsNullOrEmpty(description);

        public static void AddCategoryName(ICollection<string> messages, string name)
        {
            if (!IsCategoryName(name))
                messages.Add("Name must be provided.");
        }

        #endregion

        #region Currency

        public static bool IsCurrencyUniqueCode(string uniqueCode) => !String.IsNullOrEmpty(uniqueCode);
        public static bool IsCurrencySymbol(string symbol) => !String.IsNullOrEmpty(symbol);

        public static void AddCurrencyUniqueCode(ICollection<string> messages, string uniqueCode)
        {
            if (!IsCurrencyUniqueCode(uniqueCode))
                messages.Add("Unique Code must be provided.");
        }

        public static void AddCurrencySymbol(ICollection<string> messages, string symbol)
        {
            if (!IsCurrencySymbol(symbol))
                messages.Add("Symbol must be provided.");
        }

        #endregion

        #region ExchangeRate

        public static bool IsExchangeRate(double rate) => rate > 0;
        public static bool IsExchangeRateCurrency(string sourceCurrency, string targetCurrency) => sourceCurrency != targetCurrency;
        public static bool IsExchangeRateSourceCurrency(string currency) => !String.IsNullOrEmpty(currency);
        public static bool IsExchangeRateTargetCurrency(string currency) => !String.IsNullOrEmpty(currency);

        public static void AddExchangeRate(ICollection<string> messages, double rate)
        {
            if (!IsExchangeRate(rate))
                messages.Add("Rate must be greater than zero.");
        }

        public static void AddExchangeRateCurrency(ICollection<string> messages, string sourceCurrency, string targetCurrency)
        {
            if (!IsExchangeRateCurrency(sourceCurrency, targetCurrency))
                messages.Add("Source and target currency must be different.");
        }

        public static void AddExchangeRateSourceCurrency(ICollection<string> messages, string currency)
        {
            if (!IsExchangeRateSourceCurrency(currency))
                messages.Add("Source currency must be provided.");
        }

        public static void AddExchangeRateTargetCurrency(ICollection<string> messages, string currency)
        {
            if (!IsExchangeRateTargetCurrency(currency))
                messages.Add("Target currency must be provided.");
        }

        #endregion
    }
}
