using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Money.Services.Models
{
    /// <summary>
    /// A model of a outcome or income category.
    /// </summary>
    public class CategoryModel
    {
        /// <summary>
        /// Gets a key of the category.
        /// </summary>
        public IKey Key { get; private set; }

        /// <summary>
        /// Gets a name of the category.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a color of the category.
        /// </summary>
        public Color Color { get; private set; }

        public CategoryModel(IKey key, string name, Color color)
        {
            Ensure.Condition.NotEmptyKey(key);
            Key = key;
            Name = name;
            Color = color;
        }
    }
}
