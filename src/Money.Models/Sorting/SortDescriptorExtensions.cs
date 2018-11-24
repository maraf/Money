using Neptuo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Sorting
{
    /// <summary>
    /// A common extensions for <see cref="SortDescriptor{T}"/>.
    /// </summary>
    public static class SortDescriptorExtensions
    {
        public static SortDescriptor<T> Update<T>(this SortDescriptor<T> source, T type, ListSortDirection defaultDirection = ListSortDirection.Ascending)
            //where T : struct
        {
            if (source == null)
                return new SortDescriptor<T>(type, defaultDirection);

            if (!source.Type.Equals(type))
                return new SortDescriptor<T>(type, defaultDirection);

            if (source.Direction == ListSortDirection.Ascending)
                return new SortDescriptor<T>(type, ListSortDirection.Descending);

            return new SortDescriptor<T>(type, ListSortDirection.Ascending);
        }

        public static void Sort<TModel, TProperty>(this List<TModel> models, ListSortDirection sortDirection, Func<TModel, TProperty> selector)
            where TProperty : IComparable<TProperty>
        {
            Ensure.NotNull(models, "models");
            Ensure.NotNull(selector, "selector");
            models.Sort((x, y) =>
            {
                switch (sortDirection)
                {
                    case ListSortDirection.Ascending:
                        return selector(x).CompareTo(selector(y));
                    case ListSortDirection.Descending:
                        return selector(y).CompareTo(selector(x));
                    default:
                        throw Ensure.Exception.NotSupported(sortDirection.ToString());
                }
            });
        }
    }
}
