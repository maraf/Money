using Money;
using Neptuo.Collections.Specialized;
using Neptuo.Logging;
using Neptuo.Models.Keys;
using SimpleJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    // Worst code ever.
    public class SimpleJsonCompositeStorage : ICompositeStorage
    {
        private readonly ILog log;
        private IDictionary<string, object> storage;

        public SimpleJsonCompositeStorage(ILogFactory logFactory)
            : this(new Dictionary<string, object>(), logFactory?.Scope("Json"))
        { }

        private SimpleJsonCompositeStorage(IDictionary<string, object> storage, ILog log)
        {
            Ensure.NotNull(storage, "storage");
            this.storage = storage;
            this.log = log;
        }

        public IEnumerable<string> Keys => storage.Keys;

        public void Load(Stream input)
        {
            using (StreamReader reader = new StreamReader(input))
            {
                string value = reader.ReadToEnd();
                storage = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(value);
            }
        }

        public Task LoadAsync(Stream input)
        {
            Load(input);
            return Task.CompletedTask;
        }

        public void Store(Stream output)
        {
            string value = SimpleJson.SimpleJson.SerializeObject(storage);
            using (MemoryStream valueStream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                valueStream.CopyTo(output);

            log.Debug($"Store: Keys: '{String.Join(", ", storage.Keys)}'.");
            log.Debug($"Store: Payload-Keys: '{String.Join(", ", ((IDictionary<string, object>)storage["Payload"]).Keys)}'.");
        }

        public Task StoreAsync(Stream output)
        {
            Store(output);
            return Task.CompletedTask;
        }

        public ICompositeStorage Add(string key, object value)
        {
            log.Debug($"Add: Key: '{key}', ValueType: '{value?.GetType()?.FullName}', Value: '{value}'.");

            if (value == null)
            {
                storage[key] = null;
            }
            else if (value is GuidKey guidKey)
            {
                ICompositeStorage inner = Add(key);
                inner.Add("Type", guidKey.Type);
                if (guidKey.IsEmpty)
                    inner.Add("Guid", null);
                else
                    inner.Add("Guid", guidKey.Guid.ToString());
            }
            else if (value is StringKey stringKey)
            {
                ICompositeStorage inner = Add(key);
                inner.Add("Type", stringKey.Type);
                if (stringKey.IsEmpty)
                    inner.Add("Identifier", null);
                else
                    inner.Add("Identifier", stringKey.Identifier.ToString());
            }
            else if (value is Color color)
            {
                storage[key] = color.A + ";" + color.R + ";" + color.G + ";" + color.B;
            }
            else if (value is Price price)
            {
                ICompositeStorage inner = Add(key);
                inner.Add("Value", price.Value);
                inner.Add("Currency", price.Currency);
            }
            else
            {
                storage[key] = value;
            }

            return this;
        }

        IKeyValueCollection IKeyValueCollection.Add(string key, object value)
        {
            return Add(key, value);
        }

        public ICompositeStorage Add(string key)
        {
            log.Debug($"Add: Key: '{key}', SubStorage.");
            JsonObject innerStorage = new JsonObject();
            storage[key] = innerStorage;
            SimpleJsonCompositeStorage inner = new SimpleJsonCompositeStorage(innerStorage, log);
            return inner;
        }

        public bool TryGet(string key, out ICompositeStorage storage)
        {
            if (this.storage.TryGetValue(key, out object target))
            {
                log.Debug($"Get: Key: '{key}', RequiredType: '{typeof(ICompositeStorage).FullName}', ActualType: '{target.GetType().FullName}'.");
                if (target is JsonObject inner)
                {
                    storage = new SimpleJsonCompositeStorage(inner, log);
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

                log.Debug($"Get: Key: '{key}', RequiredType: '{typeof(T).FullName}', ActualType: '{target.GetType().FullName}'.");
                if (target is T targetValue)
                {
                    value = targetValue;
                    return true;
                }

                if (typeof(T) == typeof(int) && target.GetType() == typeof(long))
                {
                    value = (T)(object)(int)(long)target;
                    return true;
                }

                if (typeof(T) == typeof(decimal) && target.GetType() == typeof(double))
                {
                    value = (T)(object)(decimal)(double)target;
                    return true;
                }

                if (typeof(T) == typeof(IKey) && target is JsonObject json)
                {
                    string type = (string)json["Type"];
                    if (json.TryGetValue("Guid", out object rawGuid))
                    {
                        if (rawGuid == null)
                            value = (T)(object)GuidKey.Empty(type);
                        else
                            value = (T)(object)GuidKey.Create(Guid.Parse((string)rawGuid), type);

                        return true;
                    }
                    else if (json.TryGetValue("Identifier", out object rawIdentifier))
                    {
                        if (rawIdentifier == null)
                            value = (T)(object)StringKey.Empty(type);
                        else
                            value = (T)(object)StringKey.Create((string)rawIdentifier, type);

                        return true;
                    }
                }

                if (typeof(T) == typeof(Color) && target is string rawColor)
                {
                    byte[] parts = rawColor.Split(new char[] { ';' }).Select(p => Byte.Parse(p)).ToArray();
                    value = (T)(object)Color.FromArgb(parts[0], parts[1], parts[2], parts[3]);
                    log.Debug($"Get: Color: '{value}'.");
                    return true;
                }

                if (typeof(T) == typeof(Price) && target is JsonObject priceJson)
                {
                    log.Debug($"Get: Price value type: '{priceJson["Value"].GetType().FullName}'.");
                    decimal priceValue = (decimal)(double)priceJson["Value"];
                    string priceCurrency = (string)priceJson["Currency"];
                    value = (T)(object)new Price(priceValue, priceCurrency);
                    return true;
                }

                if (typeof(T) == typeof(DateTime) && target is string rawDateTime)
                {
                    if (DateTime.TryParse(rawDateTime, out DateTime dateTime))
                    {
                        value = (T)(object)dateTime;
                        return true;
                    }
                    else
                    {
                        log.Warning($"Get: Key: '{key}' not parseable to datetime from value '{rawDateTime}'.");
                        value = default(T);
                        return false;
                    }
                }
            }

            log.Debug($"Get: Key: '{key}' NOT FOUND.");
            value = default(T);
            return false;
        }
    }
}
