using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Observables.Collections
{
    public class SortableObservableCollection<T> : ObservableCollection<T>
    {
        public virtual void Sort(Func<IEnumerable<T>, IEnumerable<T>> sorter)
        {
            Ensure.NotNull(sorter, "sorter");

            IEnumerable<T> items = this.ToList();
            items = sorter(items);

            foreach (T item in items)
            {
                Remove(item);
                Add(item);
            }
        }

        /// <summary>
        /// Sorts the items of the collection in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="keySelector">A function to extract a key from an item.</param>
        public virtual void Sort<TKey>(Func<T, TKey> keySelector)
        {
            Sort(items => items.OrderBy(keySelector));
        }

        /// <summary>
        /// Sorts the items of the collection in descending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="keySelector">A function to extract a key from an item.</param>
        public virtual void SortDescending<TKey>(Func<T, TKey> keySelector)
        {
            Sort(items => items.OrderByDescending(keySelector));
        }

        /// <summary>
        /// Sorts the items of the collection in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="keySelector">A function to extract a key from an item.</param>
        /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
        public virtual void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            Sort(items => items.OrderBy(keySelector, comparer));
        }

        /// <summary>
        /// Sorts the items of the collection in descending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="keySelector">A function to extract a key from an item.</param>
        /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
        public virtual void SortDescending<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            Sort(items => items.OrderByDescending(keySelector, comparer));
        }
    }
}
