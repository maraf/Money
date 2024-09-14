using Neptuo;
using Neptuo.Formatters.Metadata;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// A model of a outcome or income category.
    /// </summary>
    public class CategoryModel : ICloneable<CategoryModel>
    {
        [CompositeVersion]
        public virtual int Version { get; set; }

        /// <summary>
        /// Gets a key of the category.
        /// </summary>
        [CompositeProperty(1)]
        public virtual IKey Key { get; set; }

        /// <summary>
        /// Gets a name of the category.
        /// </summary>
        [CompositeProperty(2)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets a description of the category.
        /// </summary>
        [CompositeProperty(3)]
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets a color of the category.
        /// </summary>
        [CompositeProperty(4)]
        public virtual Color Color { get; set; }

        /// <summary>
        /// Gets a font icon of the category.
        /// </summary>
        [CompositeProperty(5)]
        public virtual string Icon { get; set; }

        /// <summary>
        /// Gets whether the category is deleted.
        /// </summary>
        [CompositeProperty(6, Version = 2)]
        public bool IsDeleted { get; set; }

        [CompositeConstructor]
        public CategoryModel(IKey key, string name, string description, Color color, string icon)
        {
            Ensure.Condition.NotEmptyKey(key);
            Key = key;
            Name = name;
            Description = description;
            Color = color;
            Icon = icon;

            Version = 1;
        }

        [CompositeConstructor(Version = 2)]
        public CategoryModel(IKey key, string name, string description, Color color, string icon, bool isDeleted)
        {
            Ensure.Condition.NotEmptyKey(key);
            Key = key;
            Name = name;
            Description = description;
            Color = color;
            Icon = icon;
            IsDeleted = isDeleted;

            Version = 2;
        }

        public CategoryModel Clone()
        {
            if (Version == 1)
                return new CategoryModel(Key, Name, Description, Color, Icon);

            return new CategoryModel(Key, Name, Description, Color, Icon, IsDeleted);
        }
    }
}
