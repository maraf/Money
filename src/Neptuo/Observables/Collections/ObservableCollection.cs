using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Neptuo.Observables.Collections.Features;

namespace Neptuo.Observables.Collections
{
    public class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>, IRemoveAtCollection, ICountCollection
    {
        public ObservableCollection() { }

        public ObservableCollection(IEnumerable<T> items)
        {
            AddRange(items);
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
                Add(item);
        }

        public void AddRange(params T[] items)
        {
            foreach (T item in items)
                Add(item);
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        }
    }
}
