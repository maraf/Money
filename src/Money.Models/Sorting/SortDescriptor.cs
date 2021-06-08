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
    public class SortDescriptor<T> : IEquatable<SortDescriptor<T>>
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
        public SortDescriptor(T type, SortDirection direction = SortDirection.Ascending)
        {
            Type = type;
            Direction = direction;
        }

        public override bool Equals(object obj)
            => Equals(obj as SortDescriptor<T>);

        public bool Equals(SortDescriptor<T> other) => other != null &&
            EqualityComparer<T>.Default.Equals(Type, other.Type) &&
            Direction == other.Direction;

        public override int GetHashCode()
            => HashCode.Combine(Type, Direction);

        public string ToUrlString()
        {
            string name = Enum.GetName(typeof(T), Type);
            if (Direction == SortDirection.Ascending)
                return name;

            return $"{name}-Desc";
        }
    }

    public static class SortDescriptor
    {
        public static bool TryParseFromUrl<T>(string parameter, out SortDescriptor<T> value)
            where T : struct
        {
            if (String.IsNullOrEmpty(parameter))
            {
                value = null;
                return false;
            }

            string[] parts = parameter.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (!Enum.TryParse<T>(parts[0], out var enumValue))
            {
                value = null;
                return false;
            }

            if (parts.Length == 1)
            {
                value = new SortDescriptor<T>(enumValue);
                return true;
            }

            if (parts[1] == "Desc")
            {
                value = new SortDescriptor<T>(enumValue, SortDirection.Descending);
                return true;
            }

            value = null;
            return false;
        }
    }
}
