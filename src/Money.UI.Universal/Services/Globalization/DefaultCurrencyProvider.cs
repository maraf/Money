using Neptuo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services.Globalization
{
    public class DefaultCurrencyProvider : ICurrencyProvider
    {
        private Dictionary<string, (CultureInfo culture, RegionInfo region)> currencies;

        private void EnsureCurrencies()
        {
            if (currencies == null)
            {
                IEnumerable<CultureInfo> cultures = CultureHelper.GetCultures(CultureHelper.CultureTypes.AllCultures)
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

        public IEnumerable<string> GetUniqueCodes()
        {
            EnsureCurrencies();
            return currencies.Keys;
        }

        public ICurrency Get(string uniqueCode)
        {
            Ensure.NotNullOrEmpty(uniqueCode, "uniqueCode");
            EnsureCurrencies();
            if (currencies.TryGetValue(uniqueCode, out var currency))
                return new DefaultCurrency(currency.culture, currency.region);

            // TODO: Return some generic invariant culture.
            throw Ensure.Exception.NotImplemented();
        }

        #region List Helper

        private static class CultureHelper
        {
            #region Windows API

            private delegate bool EnumLocalesProcExDelegate([MarshalAs(UnmanagedType.LPWStr)]String lpLocaleString, LocaleType dwFlags, int lParam);

            [DllImport(@"kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            private static extern bool EnumSystemLocalesEx(EnumLocalesProcExDelegate pEnumProcEx, LocaleType dwFlags, int lParam, IntPtr lpReserved);

            private enum LocaleType : uint
            {
                LocaleAll = 0x00000000,             // Enumerate all named based locales
                LocaleWindows = 0x00000001,         // Shipped locales and/or replacements for them
                LocaleSupplemental = 0x00000002,    // Supplemental locales only
                LocaleAlternateSorts = 0x00000004,  // Alternate sort locales
                LocaleNeutralData = 0x00000010,     // Locales that are "neutral" (language only, region data is default)
                LocaleSpecificData = 0x00000020,    // Locales that contain language and region data
            }

            #endregion

            public enum CultureTypes : uint
            {
                SpecificCultures = LocaleType.LocaleSpecificData,
                NeutralCultures = LocaleType.LocaleNeutralData,
                AllCultures = LocaleType.LocaleWindows
            }

            public static IReadOnlyCollection<CultureInfo> GetCultures(
               CultureTypes cultureTypes)
            {
                List<CultureInfo> cultures = new List<CultureInfo>();
                EnumLocalesProcExDelegate enumCallback = (locale, flags, lParam) =>
                {
                    try
                    {
                        cultures.Add(new CultureInfo(locale));
                    }
                    catch (CultureNotFoundException)
                    {
                        // This culture is not supported by .NET (not happened so far)
                        // Must be ignored.
                    }

                    return true;
                };

                if (EnumSystemLocalesEx(enumCallback, (LocaleType)cultureTypes, 0, (IntPtr)0) == false)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new LocalesRetrievalException($"Win32 error '{errorCode}' while trying to get the Windows locales");
                }

                // Add the two neutral cultures that Windows misses 
                // (CultureInfo.GetCultures adds them also):
                //if (cultureTypes == CultureTypes.NeutralCultures || cultureTypes == CultureTypes.AllCultures)
                //{
                //    cultures.Add(new CultureInfo("zh-CHS"));
                //    cultures.Add(new CultureInfo("zh-CHT"));
                //}

                return new ReadOnlyCollection<CultureInfo>(cultures);
            }
        }

        #endregion
    }

    public class LocalesRetrievalException : Exception
    {
        public LocalesRetrievalException(string message)
            : base(message)
        {
        }
    }
}
