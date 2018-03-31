using Neptuo;
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
    public class CategoryModel
    {
        /// <summary>
        /// Gets a key of the category.
        /// </summary>
        public virtual IKey Key { get; private set; }

        /// <summary>
        /// Gets a name of the category.
        /// </summary>
        public virtual string Name { get; private set; }

        /// <summary>
        /// Gets a description of the category.
        /// </summary>
        public virtual string Description { get; private set; }

        /// <summary>
        /// Gets a color of the category.
        /// </summary>
        public virtual Color Color { get; private set; }
        
        /// <summary>
        /// Gets a font icon of the category.
        /// </summary>
        public virtual string Icon { get; private set; }

        public CategoryModel(IKey key, string name, string description, Color color, string icon)
        {
            Ensure.Condition.NotEmptyKey(key);
            Key = key;
            Name = name;
            Description = description;
            Color = color;
            Icon = icon;
        }
    }
}
