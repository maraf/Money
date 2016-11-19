using Neptuo;
using Neptuo.Activators;
using Neptuo.Models.Keys;
using Neptuo.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    /// <summary>
    /// A command facade for Money domain.
    /// </summary>
    public interface IDomainFacade
    {
        /// <summary>
        /// Gets a factory for creating prices.
        /// </summary>
        IFactory<Price, decimal> PriceFactory { get; }

        /// <summary>
        /// Creates an outcome.
        /// </summary>
        /// <param name="amount">An amount of the outcome.</param>
        /// <param name="description">A description of the outcome.</param>
        /// <param name="when">A date and time when the outcome occured.</param>
        /// <returns>Continuation task.</returns>
        Task CreateOutcomeAsync(Price amount, string description, DateTime when);
    }
}
