using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    internal static class Validator
    {
        public static bool IsOutcomeAmount(decimal amount) => amount > 0;
        public static bool IsOutcomeDecription(string description) => !String.IsNullOrEmpty(description);
        public static bool IsOutcomeCurrency(string currency) => !String.IsNullOrEmpty(currency);
        public static bool IsOutcomeCategoryKey(IKey categoryKey) => categoryKey != null && !categoryKey.IsEmpty;

        public static void AddOutcomeAmount(ICollection<string> messages, decimal amount)
        {
            if (!IsOutcomeAmount(amount))
                messages.Add("Amount must be greater than zero.");
        }

        public static void AddOutcomeDecription(ICollection<string> messages, string description)
        {
            if (!IsOutcomeDecription(description))
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
    }
}
