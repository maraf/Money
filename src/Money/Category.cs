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
using Neptuo.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// A category of outcomes or incomes.
    /// </summary>
    public class Category : AggregateRoot,
        IEventHandler<CategoryCreated>,
        IEventHandler<CategoryRenamed>,
        IEventHandler<CategoryDescriptionChanged>,
        IEventHandler<CategoryColorChanged>,
        IEventHandler<CategoryIconChanged>,
        IEventHandler<CategoryDeleted>
    {
        /// <summary>
        /// Gets a name of the category.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a color of the category.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Gets a font icon of the category.
        /// </summary>
        public string Icon { get; private set; }

        /// <summary>
        /// Gets a <c>true</c> if a category is (soft) deleted.
        /// </summary>
        public bool IsDeleted { get; private set; }

        public Category(string name, string description, Color color)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Publish(new CategoryCreated(name, color));
            Publish(new CategoryDescriptionChanged(description));
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

        public void Rename(string newName)
        {
            Ensure.NotNull(newName, "newName");

            if (newName != Name)
                Publish(new CategoryRenamed(newName, Name));
        }

        Task IEventHandler<CategoryRenamed>.HandleAsync(CategoryRenamed payload)
        {
            return UpdateState(() => Name = payload.NewName);
        }

        public void ChangeDescription(string description)
        {
            Publish(new CategoryDescriptionChanged(description));
        }

        Task IEventHandler<CategoryDescriptionChanged>.HandleAsync(CategoryDescriptionChanged payload)
        {
            return Async.CompletedTask;
        }

        public void ChangeColor(Color color)
        {
            if (color != Color)
                Publish(new CategoryColorChanged(color));
        }

        Task IEventHandler<CategoryColorChanged>.HandleAsync(CategoryColorChanged payload)
        {
            return UpdateState(() => Color = payload.Color);
        }

        public void ChangeIcon(string icon)
        {
            if (icon != Icon)
                Publish(new CategoryIconChanged(icon));
        }

        Task IEventHandler<CategoryIconChanged>.HandleAsync(CategoryIconChanged payload)
        {
            return UpdateState(() => Icon = payload.Icon);
        }

        public void Delete()
        {
            if (!IsDeleted)
                Publish(new CategoryDeleted());
        }

        public Task HandleAsync(CategoryDeleted payload)
        {
            return UpdateState(() => IsDeleted = true);
        }
    }
}
