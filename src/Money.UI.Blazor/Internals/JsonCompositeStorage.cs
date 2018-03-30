using Microsoft.AspNetCore.Blazor;
using Neptuo;
using Neptuo.Collections.Specialized;
using Neptuo.Formatters;
using Neptuo.Models.Keys;
using SimpleJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Internals
{
    public class JsonCompositeStorage : ICompositeStorage
    {
        private IDictionary<string, object> storage;

        public JsonCompositeStorage()
            : this(new Dictionary<string, object>())
        { }

        public JsonCompositeStorage(IDictionary<string, object> storage)
        {
            Ensure.NotNull(storage, "storage");
            this.storage = storage;
        }

        public IEnumerable<string> Keys => storage.Keys;

        public void Load(Stream input)
        {
            using (StreamReader reader = new StreamReader(input))
            {
                string value = reader.ReadToEnd();
                storage = JsonUtil.Deserialize<Dictionary<string, object>>(value);
            }
        }

        public Task LoadAsync(Stream input)
        {
            Load(input);
            return Task.CompletedTask;
        }

        public void Store(Stream output)
        {
            string value = JsonUtil.Serialize(storage);
            using (MemoryStream valueStream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                valueStream.CopyTo(output);
        }

        public Task StoreAsync(Stream output)
        {
            Store(output);
            return Task.CompletedTask;
        }

        public ICompositeStorage Add(string key, object value)
        {
            storage[key] = value;
            return this;
        }

        IKeyValueCollection IKeyValueCollection.Add(string key, object value)
        {
            return Add(key, value);
        }

        public ICompositeStorage Add(string key)
        {
            JsonCompositeStorage inner = new JsonCompositeStorage();
            storage[key] = inner.storage;
            return inner;
        }

        public bool TryGet(string key, out ICompositeStorage storage)
        {
            if (this.storage.TryGetValue(key, out object target))
            {
                Console.WriteLine($"JSON: Key: {key}, RequiredType: {typeof(ICompositeStorage).FullName}, ActualType: {target.GetType().FullName}.");
                if (target is JsonObject inner)
                {
                    storage = new JsonCompositeStorage(inner);
                    return true;
                }
            }

            storage = null;
            return false;
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (storage.TryGetValue(key, out object target))
            {
                if (target == null)
                {
                    value = default(T);
                    return true;
                }

                Console.WriteLine($"JSON: Key: {key}, RequiredType: {typeof(T).FullName}, ActualType: {target.GetType().FullName}.");
                if (typeof(T) == typeof(int) && target.GetType() == typeof(long))
                {
                    value = (T)(object)(int)(long)target;
                    return true;
                }

                if (typeof(T) == typeof(IKey) && target is JsonObject json)
                {
                    string type = (string)json["Type"];
                    if (json.TryGetValue("Guid", out object rawGuid))
                    {
                        value = (T)(object)GuidKey.Create(Guid.Parse((string)rawGuid), type);
                        return true;
                    }
                    else if (json.TryGetValue("Identifier", out object rawIdentifier))
                    {
                        value = (T)(object)StringKey.Create((string)rawIdentifier, type);
                        return true;
                    }
                }

                if (typeof(T) == typeof(Color) && target is string rawValue)
                {
                    byte[] parts = rawValue.Split(new char[] { ';' }).Select(p => Byte.Parse(p)).ToArray();
                    value = (T)(object)Color.FromArgb(parts[0], parts[1], parts[2], parts[3]);
                    Console.WriteLine(value);
                    return true;
                }

                if (target is T targetValue)
                {
                    value = targetValue;
                    return true;
                }
            }

            Console.WriteLine($"JSON: Key: {key} NOT FOUND.");
            value = default(T);
            return false;
        }
    }
}
