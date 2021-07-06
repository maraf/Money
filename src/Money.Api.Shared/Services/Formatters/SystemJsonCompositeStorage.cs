using Money;
using Money.Services;
using Neptuo.Collections.Specialized;
using Neptuo.Logging;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JsonElement = System.Text.Json.JsonElement;
using JsonProperty = System.Text.Json.JsonProperty;

namespace Neptuo.Formatters
{
    // Worst code ever.
    public class SystemJsonCompositeStorage : ICompositeStorage
    {
        private readonly ILog log;
        private readonly Json json;
        private Dictionary<string, object> storage;

        public SystemJsonCompositeStorage(ILogFactory logFactory, Json json)
            : this(new Dictionary<string, object>(), logFactory?.Scope("Json"), json)
        {
        }

        private SystemJsonCompositeStorage(Dictionary<string, object> storage, ILog log, Json json)
        {
            Ensure.NotNull(storage, "storage");
            Ensure.NotNull(json, "json");
            this.storage = storage;
            this.log = log;
            this.json = json;
        }

        public IEnumerable<string> Keys => storage.Keys;

        public void Load(Stream input)
        {
            using (StreamReader reader = new StreamReader(input))
            {
                string value = reader.ReadToEnd();
                storage = json.Deserialize<Dictionary<string, object>>(value);
                log.Debug($"Load: '{value}'.");
            }
        }

        public Task LoadAsync(Stream input)
        {
            Load(input);
            return Task.CompletedTask;
        }

        public void Store(Stream output)
        {
            string value = json.Serialize(storage);
            using (MemoryStream valueStream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                valueStream.CopyTo(output);

            if (log.IsDebugEnabled())
            {
                log.Debug($"Store: Keys: '{String.Join(", ", storage.Keys)}'.");
                if (storage.ContainsKey("Payload"))
                    log.Debug($"Store: Payload-Keys: '{String.Join(", ", ((IDictionary<string, object>)storage["Payload"]).Keys)}'.");
            }
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
            Dictionary<string, object> innerStorage = new Dictionary<string, object>();
            storage[key] = innerStorage;
            SystemJsonCompositeStorage inner = new SystemJsonCompositeStorage(innerStorage, log, json);
            return inner;
        }

        public bool TryGet(string key, out ICompositeStorage storage)
        {
            if (this.storage.TryGetValue(key, out object target))
            {
                log.Debug($"GetStorage: Key: '{key}', RequiredType: '{typeof(ICompositeStorage).FullName}', ActualType: '{target.GetType().FullName}'.");

                if (target is Dictionary<string, object> inner)
                {
                    storage = new SystemJsonCompositeStorage(inner, log, json);
                    return true;
                }

                if (target is JsonElement element)
                {
                    log.Debug($"JsonElement: '{element}'.");

                    inner = new Dictionary<string, object>();
                    foreach (JsonProperty property in element.EnumerateObject())
                    {
                        log.Debug($"Adding key '{property.Name}': '{property.Value}'.");
                        inner[property.Name] = property.Value;
                    }

                    storage = new SystemJsonCompositeStorage(inner, log, json);
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

                JsonElement element = (JsonElement)target;
                if (element.ValueKind == JsonValueKind.Null)
                {
                    value = default(T);
                    return true;
                }

                if (typeof(T) == typeof(string))
                {
                    value = (T)(object)element.GetString();
                    return true;
                }

                if (typeof(T) == typeof(int))
                {
                    value = (T)(object)element.GetInt32();
                    return true;
                }

                if (typeof(T) == typeof(long))
                {
                    value = (T)(object)element.GetInt64();
                    return true;
                }

                if (typeof(T) == typeof(decimal))
                {
                    value = (T)(object)element.GetDecimal();
                    return true;
                }

                if (typeof(T) == typeof(double))
                {
                    value = (T)(object)element.GetDouble();
                    return true;
                }

                if (typeof(T) == typeof(bool))
                {
                    value = (T)(object)element.GetBoolean();
                    return true;
                }

                if (typeof(T) == typeof(IKey))
                {
                    if (element.ValueKind == JsonValueKind.Null)
                    {
                        value = default(T);
                        return true;
                    }

                    string type = element.GetProperty("Type").GetString();
                    if (element.TryGetProperty("Guid", out JsonElement rawGuid))
                    {
                        string rawGuidValue = rawGuid.GetString();
                        if (rawGuidValue == null)
                            value = (T)(object)GuidKey.Empty(type);
                        else
                            value = (T)(object)GuidKey.Create(Guid.Parse(rawGuidValue), type);

                        return true;
                    }
                    else if (element.TryGetProperty("Identifier", out JsonElement rawIdentifier))
                    {
                        string rawIdentifierValue = rawIdentifier.GetString();
                        if (rawIdentifierValue == null)
                            value = (T)(object)StringKey.Empty(type);
                        else
                            value = (T)(object)StringKey.Create(rawIdentifierValue, type);

                        return true;
                    }
                }

                if (typeof(T) == typeof(Color))
                {
                    byte[] parts = element.GetString().Split(new char[] { ';' }).Select(p => Byte.Parse(p)).ToArray();
                    value = (T)(object)Color.FromArgb(parts[0], parts[1], parts[2], parts[3]);
                    log.Debug($"Get: Color: '{value}'.");
                    return true;
                }

                if (typeof(T) == typeof(Price))
                {
                    log.Debug($"Get: Price value type: '{element.GetProperty("Value").GetType().FullName}'.");
                    decimal priceValue = element.GetProperty("Value").GetDecimal();
                    string priceCurrency = element.GetProperty("Currency").GetString();
                    value = (T)(object)new Price(priceValue, priceCurrency);
                    return true;
                }

                if (typeof(T) == typeof(DateTime))
                {
                    string rawDateTime = element.GetString();
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

            log.Debug($"Get: Key: '{key}' NOT FOUND. Storage: '{JsonSerializer.Serialize(storage)}'.");
            value = default(T);
            return false;
        }
    }
}
