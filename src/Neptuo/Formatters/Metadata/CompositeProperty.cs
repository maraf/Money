using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    /// <summary>
    /// A single property.
    /// </summary>
    public class CompositeProperty
    {
        /// <summary>
        /// The index of the property in constructor.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The type of the property value.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// The expression to get a value from an instance.
        /// </summary>
        public Func<object, object> Getter { get; private set; }

        /// <summary>
        /// The expression to set a value to an instance.
        /// This can be <c>null</c> for immutable properties.
        /// </summary>
        public Action<object, object> Setter { get; private set; }

        /// <summary>
        /// Creates new instance without <see cref="Setter"/>.
        /// </summary>
        /// <param name="index">The index of the property in constructor.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="type">The type of the property value.</param>
        /// <param name="getter">The expression to get a value from an instance.</param>
        public CompositeProperty(int index, string name, Type type, Func<object, object> getter)
        {
            Ensure.PositiveOrZero(index, "index");
            Ensure.NotNullOrEmpty(name, "name");
            Ensure.NotNull(type, "type");
            Ensure.NotNull(getter, "getter");
            Index = index;
            Name = name;
            Type = type;
            Getter = getter;
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="index">The index of the property in constructor.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="type">The type of the property value.</param>
        /// <param name="getter">The expression to get a value from an instance.</param>
        /// <param name="setter">The expression to set a value to an instance.</param>
        public CompositeProperty(int index, string name, Type type, Func<object, object> getter, Action<object, object> setter)
            : this(index, name, type, getter)
        {
            Ensure.NotNull(setter, "setter");
            Setter = setter;
        }
    }
}
