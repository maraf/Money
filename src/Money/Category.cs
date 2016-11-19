using Neptuo.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Events;
using Neptuo.Models.Keys;
using Neptuo;
using Money.Events;
using Neptuo.Events.Handlers;

namespace Money
{
    /// <summary>
    /// A category of outcomes or incomes.
    /// </summary>
    public class Category : AggregateRoot,
        IEventHandler<CategoryCreated>
    {
        /// <summary>
        /// Gets a name of the category.
        /// </summary>
        public string Name { get; private set; }

        public Category(string name)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Publish(new CategoryCreated(name));
        }

        public Category(IKey key, IEnumerable<IEvent> events) 
            : base(key, events)
        { }

        Task IEventHandler<CategoryCreated>.HandleAsync(CategoryCreated payload)
        {
            return UpdateState(() => Name = payload.Name);
        }
    }
}
