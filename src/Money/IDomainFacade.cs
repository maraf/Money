using Neptuo.Activators;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Money
{
    /// <summary>
    /// A command facade for Money domain.
    /// </summary>
    public interface IDomainFacade
    {
        /// <summary>
        /// Creates an category.
        /// </summary>
        /// <param name="name">A name of the category.</param>
        /// <param name="color">A color of the category.</param>
        /// <returns>Continuation task. The result contains a key of the new category.</returns>
        Task<IKey> CreateCategoryAsync(string name, Color color);

        /// <summary>
        /// Creates an outcome.
        /// </summary>
        /// <param name="amount">An amount of the outcome.</param>
        /// <param name="description">A description of the outcome.</param>
        /// <param name="when">A date and time when the outcome occured.</param>
        /// <returns>Continuation task. The result contains a key of the new outcome.</returns>
        Task<IKey> CreateOutcomeAsync(Price amount, string description, DateTime when, IKey categoryKey);

        /// <summary>
        /// Adds <paramref name="categoryKey"/> to the <paramref name="outcomeKey"/>.
        /// </summary>
        /// <param name="outcomeKey">A key of the outcome to add category to.</param>
        /// <param name="categoryKey">A key of the category to add outcome to.</param>
        /// <returns>Continuation task.</returns>
        Task AddOutcomeCategoryAsync(IKey outcomeKey, IKey categoryKey);

        /// <summary>
        /// Changes an <paramref name="amount"/> of the outcome with <paramref name="key"/>.
        /// </summary>
        /// <param name="outcomeKey">A key of the outcome to modify.</param>
        /// <param name="amount">A new outcome value.</param>
        /// <returns>Continuation task.</returns>
        Task ChangeOutcomeAmountAsync(IKey outcomeKey, Price amount);

        /// <summary>
        /// Changes a <paramref name="description"/> of the outcome with <paramref name="key"/>.
        /// </summary>
        /// <param name="outcomeKey">A key of the outcome to modify.</param>
        /// <param name="description">A new description of the outcome.</param>
        /// <returns>Continuation task.</returns>
        Task ChangeOutcomeDescriptionAsync(IKey outcomeKey, string description);

        /// <summary>
        /// Changes a <paramref name="when"/> of the outcome with <paramref name="key"/>.
        /// </summary>
        /// <param name="outcomeKey">A key of the outcome to modify.</param>
        /// <param name="when">A date when the outcome occured.</param>
        /// <returns>Continuation task.</returns>
        Task ChangeOutcomeWhenAsync(IKey outcomeKey, DateTime when);

        /// <summary>
        /// Deletes an outcome with <paramref name="outcomeKey"/>.
        /// </summary>
        /// <param name="outcomeKey">A key of the outcome to delete.</param>
        /// <returns>Continuation task.</returns>
        Task DeleteOutcomeAsync(IKey outcomeKey);

        /// <summary>
        /// Renames a category with a key <paramref name="categoryKey"/>.
        /// </summary>
        /// <param name="categoryKey">A key of the category to rename.</param>
        /// <param name="newName">A new name of the category.</param>
        /// <returns>Continuation task.</returns>
        Task RenameCategoryAsync(IKey categoryKey, string newName);

        /// <summary>
        /// Changes a description of a category with a key <paramref name="categoryKey"/>.
        /// </summary>
        /// <param name="categoryKey">A key of the category.</param>
        /// <param name="description">A new description of the category.</param>
        /// <returns>Continuation task.</returns>
        Task ChangeCategoryDescriptionAsync(IKey categoryKey, string description);

        /// <summary>
        /// Changes a color of a category with a key <paramref name="categoryKey"/>.
        /// </summary>
        /// <param name="categoryKey">A key of the category.</param>
        /// <param name="color">A new color of the category.</param>
        /// <returns>Continuation task.</returns>
        Task ChangeCategoryColorAsync(IKey categoryKey, Color color);

        /// <summary>
        /// Changes a font icon for a category with a key <paramref name="categoryKey"/>.
        /// </summary>
        /// <param name="categoryKey">A key of the category.</param>
        /// <param name="icon">A font icon of the category</param>
        /// <returns>Continuation task.</returns>
        Task ChangeCategoryIconAsync(IKey categoryKey, string icon);

        /// <summary>
        /// Deletes (soft) category with <paramref name="categoryKey"/>.
        /// </summary>
        /// <param name="categoryKey">A key of the category.</param>
        /// <returns>Continuation task.</returns>
        Task DeleteCategoryAsync(IKey categoryKey);

        /// <summary>
        /// Creates a new currency with <paramref name="uniqueCode"/> as a unique identifier.
        /// </summary>
        /// <param name="uniqueCode">An unique code of the new currency.</param>
        /// <param name="symbol">A symbol of the new currency</param>
        /// <returns>Continuation task.</returns>
        Task CreateCurrencyAsync(string uniqueCode, string symbol);

        /// <summary>
        /// Sets a <paramref name="uniqueCode"/> as a default currency.
        /// </summary>
        /// <param name="uniqueCode">An unique code of the currency.</param>
        /// <returns>Continuation task.</returns>
        Task SetCurrencyAsDefaultAsync(string uniqueCode);

        /// <summary>
        /// Changes a <paramref name="symbol"/> of the currency with <paramref name="uniqueCode"/>.
        /// </summary>
        /// <param name="uniqueCode">An unique code of the currency to change symbol for.</param>
        /// <param name="symbol">A new symbol for the currency.</param>
        /// <returns>Continuation task.</returns>
        Task ChangeCurrencySymbolAsync(string uniqueCode, string symbol);

        /// <summary>
        /// Sets an exchange rate from <paramref name="sourceUniqueCode"/> to <paramref name="targetUniqueCode"/>.
        /// It is valid from <paramref name="validFrom"/> with exchange <paramref name="rate"/>.
        /// </summary>
        /// <param name="sourceUniqueCode">A source currency unique code.</param>
        /// <param name="targetUniqueCode">A target currency unique code.</param>
        /// <param name="validFrom">A date from which the exchange rate is valid.</param>
        /// <param name="rate">An exchange rate.</param>
        /// <returns>Continuation task.</returns>
        Task SetExchangeRateAsync(string sourceUniqueCode, string targetUniqueCode, DateTime validFrom, double rate);

        /// <summary>
        /// Deletes (soft) currency with <paramref name="uniqueCode"/>.
        /// </summary>
        /// <param name="uniqueCode">An unique code for the currency to delete.</param>
        /// <returns>Continuation task.</returns>
        Task DeleteCurrencyAsync(string uniqueCode);
    }
}
