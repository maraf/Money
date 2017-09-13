using Neptuo;
using Neptuo.Collections.Specialized;
using Neptuo.Converters;
using Neptuo.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Money.Services.Settings
{
    public class ApplicationDataCompositeStorage : ICompositeStorage
    {
        private readonly ApplicationDataContainer root;
        private readonly IConverterRepository converters = Converts.Repository;
        
        public ApplicationDataCompositeStorage(ApplicationDataContainer root)
        {
            Ensure.NotNull(root, "root");
            this.root = root;
        }

        public void Load(Stream input)
        { }

        public Task LoadAsync(Stream input)
        {
            return Task.CompletedTask;
        }

        public void Store(Stream output)
        { }

        public Task StoreAsync(Stream output)
        {
            return Task.CompletedTask;
        }

        public IEnumerable<string> Keys
        {
            get { return Enumerable.Concat(root.Values.Keys, root.Containers.Keys); }
        }

        IKeyValueCollection IKeyValueCollection.Add(string key, object value) => Add(key, value);

        public ICompositeStorage Add(string key, object value)
        {
            string rawValue = null;
            if (value != null)
            {
                if (converters.TryConvert(value.GetType(), typeof(string), value, out object converted))
                    rawValue = (string)converted;
                else
                    throw Ensure.Exception.NotSupportedConversion(typeof(string), value);
            }

            root.Values[key] = rawValue;
            return this;
        }

        public ICompositeStorage Add(string key)
        {
            ApplicationDataContainer container = root.CreateContainer(key, ApplicationDataCreateDisposition.Always);
            return new ApplicationDataCompositeStorage(container);
        }

        public bool TryGet(string key, out ICompositeStorage storage)
        {
            if (root.Containers.TryGetValue(key, out ApplicationDataContainer container))
            {
                storage = new ApplicationDataCompositeStorage(container);
                return true;
            }

            storage = null;
            return false;
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (root.Values.TryGetValue(key, out object rawValue))
            {
                if (rawValue != null && converters.TryConvert(rawValue.GetType(), typeof(T), rawValue, out object converted))
                {
                    value = (T)converted;
                    return true;
                }

                value = default(T);
                return true;
            }

            value = default(T);
            return false;
        }
    }
}
