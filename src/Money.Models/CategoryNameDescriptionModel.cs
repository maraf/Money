using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// A model of category name and description.
    /// </summary>
    public class CategoryNameDescriptionModel
    {
        /// <summary>
        /// Gets a category name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a category description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="name">A category name.</param>
        /// <param name="description">A category description.</param>
        public CategoryNameDescriptionModel(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
