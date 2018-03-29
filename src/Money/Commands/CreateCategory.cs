using Neptuo;
using Neptuo.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    /// <summary>
    /// Creates an category.
    /// </summary>
    public class CreateCategory : Command
    {
        /// <summary>
        /// Gets a name of the category.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a description of the category.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a color of the category.
        /// </summary>
        public Color Color { get; private set; }

        public CreateCategory(string name, string description, Color color)
        {
            Ensure.NotNull(name, "name");
            Ensure.NotNull(description, "description");
            Ensure.NotNull(color, "color");
            Name = name;
            Description = description;
            Color = color;
        }
    }
}
