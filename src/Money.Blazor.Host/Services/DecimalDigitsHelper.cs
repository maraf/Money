using System;

namespace Money.Services
{
    public static class DecimalDigitsHelper
    {
        /// <summary>
        /// Returns the effective number of decimal digits to use for display/input.
        /// If the value has a fractional part, returns at least 2 digits.
        /// </summary>
        public static int GetEffectiveDecimalDigits(decimal value, int userDecimalDigits)
        {
            if (value != Math.Truncate(value))
                return Math.Max(userDecimalDigits, 2);

            return userDecimalDigits;
        }
    }
}
