using Money.Events;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Models.Domains;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// A model of the include
    /// </summary>
    public class Income : AggregateRoot,
        IEventHandler<IncomeCreated>,
        IEventHandler<IncomeAmountChanged>,
        IEventHandler<IncomeDescriptionChanged>,
        IEventHandler<IncomeWhenChanged>,
        IEventHandler<IncomeDeleted>
    {
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// Gets an amount of the income.
        /// </summary>
        public Price Amount { get; private set; }

        /// <summary>
        /// Gets a description of the income.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a date when the income occured.
        /// </summary>
        public DateTime When { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="amount">An amount of the income.</param>
        /// <param name="description">A description of the income.</param>
        /// <param name="when">A date when the income occured.</param>
        public Income(Price amount, string description, DateTime when)
        {
            Publish(new IncomeCreated(amount, description, when));
        }

        public Income(IKey key, IEnumerable<IEvent> events)
            : base(key, events)
        { }

        Task IEventHandler<IncomeCreated>.HandleAsync(IncomeCreated payload)
        {
            return UpdateState(() =>
            {
                Amount = payload.Amount;
                Description = payload.Description;
                When = payload.When;
            });
        }

        private void EnsureNotDeleted()
        {
            if (IsDeleted)
                throw new IncomeAlreadyDeletedException();
        }

        public void ChangeAmount(Price amount)
        {
            EnsureNotDeleted();

            if (Amount != amount)
                Publish(new IncomeAmountChanged(Amount, amount));
        }

        Task IEventHandler<IncomeAmountChanged>.HandleAsync(IncomeAmountChanged payload)
        {
            return UpdateState(() =>
            {
                Amount = payload.NewValue;
            });
        }

        public void ChangeDescription(string description)
        {
            EnsureNotDeleted();

            if (Description != description)
                Publish(new IncomeDescriptionChanged(description));
        }

        Task IEventHandler<IncomeDescriptionChanged>.HandleAsync(IncomeDescriptionChanged payload)
        {
            return UpdateState(() =>
            {
                Description = payload.Description;
            });
        }

        public void ChangeWhen(DateTime when)
        {
            EnsureNotDeleted();

            if (When != when)
                Publish(new IncomeWhenChanged(when));
        }

        Task IEventHandler<IncomeWhenChanged>.HandleAsync(IncomeWhenChanged payload)
        {
            return UpdateState(() =>
            {
                When = payload.When;
            });
        }

        public void Delete()
        {
            EnsureNotDeleted();
            Publish(new IncomeDeleted());
        }

        Task IEventHandler<IncomeDeleted>.HandleAsync(IncomeDeleted payload)
        {
            return UpdateState(() =>
            {
                IsDeleted = true;
            });
        }
    }
}
