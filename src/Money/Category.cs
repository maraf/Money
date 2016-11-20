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
using Windows.UI;

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

        /// <summary>
        /// Gets a color of the category.
        /// </summary>
        public Color Color { get; private set; }

        public Category(string name, Color color)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Publish(new CategoryCreated(name, color));
        }

        public Category(IKey key, IEnumerable<IEvent> events)
            : base(key, events)
        { }

        Task IEventHandler<CategoryCreated>.HandleAsync(CategoryCreated payload)
        {
            return UpdateState(() =>
            {
                Name = payload.Name;
                Color = payload.Color;
            });
        }
    }
}
