using Money.Services.Models;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Models.Keys;
using Neptuo.Models.Repositories;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Money.Services
{
    /// <summary>
    /// A command facade for Money domain.
    /// </summary>
    public interface IDomainFacade : IQueryDispatcher
    {
        /// <summary>
        /// Gets a factory for creating prices.
        /// </summary>
        IFactory<Price, decimal> PriceFactory { get; }

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
    }
}
