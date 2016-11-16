using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Observables.Collections
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private Dictionary<TKey, TValue> storage = new Dictionary<TKey, TValue>();

        public void Add(TKey key, TValue value)
        {
            storage.Add(key, value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, key));
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Keys"));
            OnPropertyChanged(new PropertyChangedEventArgs("Values"));
        }

        public bool ContainsKey(TKey key)
        {
            return storage.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return storage.Keys; }
        }

        public bool Remove(TKey key)
        {
            bool result = storage.Remove(key);
            if (result)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, key));
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Keys"));
                OnPropertyChanged(new PropertyChangedEventArgs("Values"));
            }

            return result;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return storage.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return storage.Values; }
        }

        public TValue this[TKey key]
        {
            get { return storage[key]; }
            set
            {
                if (ContainsKey(key))
                    storage[key] = value;
                else
                    Add(key, value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Keys));
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Keys"));
            OnPropertyChanged(new PropertyChangedEventArgs("Values"));
            storage.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return storage.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw Ensure.Exception.NotImplemented();
        }

        public int Count
        {
            get { return storage.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return storage.GetEnumerator();
        }

        #region Notify***Changed

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        #endregion
    }
}
