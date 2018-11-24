using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Sorting
{
    /// <summary>
    /// A descriptor of item sorting.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortDescriptor<T>
        //where T : struct
    {
        /// <summary>
        /// Gets a selected sorting type.
        /// </summary>
        public T Type { get; private set; }

        /// <summary>
        /// Gets a direction of sorting.
        /// </summary>
        public ListSortDirection Direction { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="type">A selected sorting type.</param>
        /// <param name="direction">A direction of sorting.</param>
        public SortDescriptor(T type, ListSortDirection direction)
        {
            Type = type;
            Direction = direction;
        }
    }
}
