using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    /// <summary>
    /// A descriptor of item sorting.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortDescriptor<T>
        where T : struct
    {
        /// <summary>
        /// Gets a selected sorting type.
        /// </summary>
        public T Type { get; private set; }

        /// <summary>
        /// Gets a direction of sorting.
        /// </summary>
        public SortDirection Direction { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="type">A selected sorting type.</param>
        /// <param name="direction">A direction of sorting.</param>
        public SortDescriptor(T type, SortDirection direction)
        {
            Type = type;
            Direction = direction;
        }
    }

    public static class SortDescriptorExtensions
    {
        public static SortDescriptor<T> Update<T>(this SortDescriptor<T> source, T type, SortDirection defaultDirection = SortDirection.Ascending)
            where T : struct
        {
            if (source == null)
                return new SortDescriptor<T>(type, defaultDirection);

            if (!source.Type.Equals(type))
                return new SortDescriptor<T>(type, defaultDirection);

            if (source.Direction == SortDirection.Ascending)
                return new SortDescriptor<T>(type, SortDirection.Descending);

            return new SortDescriptor<T>(type, SortDirection.Ascending);
        }
    }
}
