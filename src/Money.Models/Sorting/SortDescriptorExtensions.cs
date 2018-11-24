using Neptuo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Sorting
{
    /// <summary>
    /// A common extensions for <see cref="SortDescriptor{T}"/>.
    /// </summary>
    public static class SortDescriptorExtensions
    {
        public static SortDescriptor<T> Update<T>(this SortDescriptor<T> source, T type, SortDirection defaultDirection = SortDirection.Ascending)
            //where T : struct
        {
            if (source == null)
                return new SortDescriptor<T>(type, defaultDirection);

            if (!source.Type.Equals(type))
                return new SortDescriptor<T>(type, defaultDirection);

            if (source.Direction == SortDirection.Ascending)
                return new SortDescriptor<T>(type, SortDirection.Descending);

            return new SortDescriptor<T>(type, SortDirection.Ascending);
        }

        public static void Sort<TModel, TProperty>(this List<TModel> models, SortDirection sortDirection, Func<TModel, TProperty> selector)
            where TProperty : IComparable<TProperty>
        {
            Ensure.NotNull(models, "models");
            Ensure.NotNull(selector, "selector");
            models.Sort((x, y) =>
            {
                switch (sortDirection)
                {
                    case SortDirection.Ascending:
                        return selector(x).CompareTo(selector(y));
                    case SortDirection.Descending:
                        return selector(y).CompareTo(selector(x));
                    default:
                        throw Ensure.Exception.NotSupported(sortDirection.ToString());
                }
            });
        }

        public static IQueryable<TModel> OrderBy<TModel, TProperty>(this IQueryable<TModel> models, SortDirection sortDirection, Expression<Func<TModel, TProperty>> selector)
            where TProperty : IComparable<TProperty>
        {
            Ensure.NotNull(models, "models");
            Ensure.NotNull(selector, "selector");

            switch (sortDirection)
            {
                case SortDirection.Ascending:
                    return models.OrderBy(selector);
                case SortDirection.Descending:
                    return models.OrderByDescending(selector);
                default:
                    throw Ensure.Exception.NotSupported(sortDirection.ToString());
            }
        }
    }
}
