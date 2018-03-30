using Neptuo.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    /// <summary>
    /// Newtonsoft.Json implementation of <see cref="ICompositeStorage"/>.
    /// </summary>
    public class JsonCompositeStorage : ICompositeStorage
    {
        private JObject root;

        private readonly Formatting formatting;
        private readonly JsonLoadSettings loadSettings;

        /// <summary>
        /// Creates new instance with default settings and <see cref="Formatting.None"/> for serialization.
        /// </summary>
        public JsonCompositeStorage()
            : this(Formatting.None)
        { }

        /// <summary>
        /// Creates new instance with <paramref name="formatting" />.
        /// </summary>
        /// <param name="formatting">The indention formatting for serialization.</param>
        public JsonCompositeStorage(Formatting formatting)
        {
            this.loadSettings = new JsonLoadSettings();
            this.formatting = formatting;
            root = new JObject();
        }

        private JsonCompositeStorage(JObject root, Formatting formatting)
            : this(formatting)
        {
            Ensure.NotNull(root, "root");
            this.root = root;
        }

        public async Task LoadAsync(Stream input)
        {
            Ensure.NotNull(input, "input");

            using (StreamReader reader = new StreamReader(input))
                root = JObject.Parse(await reader.ReadToEndAsync().ConfigureAwait(false), loadSettings);
        }

        public void Load(Stream input)
        {
            Ensure.NotNull(input, "input");

            using (StreamReader reader = new StreamReader(input))
                root = JObject.Parse(reader.ReadToEnd(), loadSettings);
        }

        public async Task StoreAsync(Stream output)
        {
            Ensure.NotNull(output, "output");

            using (StreamWriter writer = new StreamWriter(output, Encoding.UTF8, 1024, true))
                await writer.WriteAsync(root.ToString(formatting)).ConfigureAwait(false);
        }

        public void Store(Stream output)
        {
            Ensure.NotNull(output, "output");

            using (StreamWriter writer = new StreamWriter(output, Encoding.UTF8, 1024, true))
                writer.Write(root.ToString(formatting));
        }

        public IEnumerable<string> Keys
        {
            get
            {
                foreach (var item in root)
                    yield return item.Key;
            }
        }

        public ICompositeStorage Add(string key, object value)
        {
            object targetValue = null;
            JToken jValue = null;
            if (value == null)
                jValue = null;
            else if (Converts.Try(value.GetType(), typeof(JToken), value, out targetValue))
                jValue = (JToken)targetValue;

            root[key] = jValue;
            return this;
        }

        public ICompositeStorage Add(string key)
        {
            JObject child = new JObject();
            root[key] = child;
            return new JsonCompositeStorage(child, formatting);
        }

        IKeyValueCollection IKeyValueCollection.Add(string key, object value)
        {
            return Add(key, value);
        }

        public bool TryGet<T>(string key, out T value)
        {
            JToken jToken;
            if (root.TryGetValue(key, out jToken))
                return Converts.Try<JToken, T>(jToken, out value);

            value = default(T);
            return false;
        }

        public bool TryGet(string key, out ICompositeStorage storage)
        {
            JToken jToken;
            JObject jObject;
            if (root.TryGetValue(key, out jToken) && (jObject = jToken as JObject) != null)
            {
                storage = new JsonCompositeStorage(jObject, formatting);
                return true;
            }

            storage = null;
            return false;
        }
    }
}
