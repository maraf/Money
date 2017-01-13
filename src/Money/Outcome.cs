using Money.Events;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Models.Domains;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// A model of outcome.
    /// </summary>
    public class Outcome : AggregateRoot,
        IEventHandler<OutcomeCreated>,
        IEventHandler<OutcomeCategoryAdded>,
        IEventHandler<OutcomeAmountChanged>,
        IEventHandler<OutcomeDescriptionChanged>,
        IEventHandler<OutcomeWhenChanged>
    {
        /// <summary>
        /// Gets an amount of the outcome.
        /// </summary>
        public Price Amount { get; private set; }

        /// <summary>
        /// Gets a description of the outcome.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a date when the outcome occured.
        /// </summary>
        public DateTime When { get; private set; }

        /// <summary>
        /// Gets a collection of assigned categories.
        /// </summary>
        public HashSet<IKey> CategoryKeys { get; private set; } = new HashSet<IKey>();

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="amount">An amount of the outcome.</param>
        /// <param name="description">A description of the outcome.</param>
        /// <param name="when">A date when the outcome occured.</param>
        /// <param name="categoryKey">A category where the outcome is created.</param>
        public Outcome(Price amount, string description, DateTime when, IKey categoryKey)
        {
            Publish(new OutcomeCreated(amount, description, when, categoryKey));
        }

        public Outcome(IKey key, IEnumerable<IEvent> events)
            : base(key, events)
        { }

        Task IEventHandler<OutcomeCreated>.HandleAsync(OutcomeCreated payload)
        {
            return UpdateState(() =>
            {
                Amount = payload.Amount;
                Description = payload.Description;
                When = payload.When;
                CategoryKeys.Add(payload.CategoryKey);
            });
        }

        /// <summary>
        /// Tries to add <paramref name="categoryKey"/> to a list of categories.
        /// </summary>
        /// <param name="categoryKey"></param>
        /// <exception cref="OutcomeAlreadyHasCategoryException">When the outcome already has category <paramref name="categoryKey"/> assigned.</exception>
        public void AddCategory(IKey categoryKey)
        {
            if (CategoryKeys.Contains(categoryKey))
                throw new OutcomeAlreadyHasCategoryException(categoryKey);

            Publish(new OutcomeCategoryAdded(categoryKey));
        }

        Task IEventHandler<OutcomeCategoryAdded>.HandleAsync(OutcomeCategoryAdded payload)
        {
            return UpdateState(() => CategoryKeys.Add(payload.CategoryKey));
        }

        public void ChangeAmount(Price amount)
        {
            if (Amount != amount)
                Publish(new OutcomeAmountChanged(Amount, amount));
        }

        Task IEventHandler<OutcomeAmountChanged>.HandleAsync(OutcomeAmountChanged payload)
        {
            return UpdateState(() => Amount = payload.NewValue);
        }

        public void ChangeDescription(string description)
        {
            if (Description != description)
                Publish(new OutcomeDescriptionChanged(description));
        }

        Task IEventHandler<OutcomeDescriptionChanged>.HandleAsync(OutcomeDescriptionChanged payload)
        {
            return UpdateState(() => Description = payload.Description);
        }

        public void ChangeWhen(DateTime when)
        {
            if (When != when)
                Publish(new OutcomeWhenChanged(when));
        }

        Task IEventHandler<OutcomeWhenChanged>.HandleAsync(OutcomeWhenChanged payload)
        {
            return UpdateState(() => When = payload.When);
        }
    }
}
