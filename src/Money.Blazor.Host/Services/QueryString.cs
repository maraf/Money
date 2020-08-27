using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Neptuo;
using Neptuo.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    /// <summary>
    /// A current parsed query string provider.
    /// </summary>
    public class QueryString : DisposableBase, IReadOnlyKeyValueCollection
    {
        private readonly NavigationManager manager;
        private KeyValueCollection parameters;

        public QueryString(NavigationManager manager)
        {
            Ensure.NotNull(manager, "manager");
            this.manager = manager;

            manager.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
            => parameters = null;

        private void EnsureParameters()
        {
            if (parameters == null)
            {
                parameters = new KeyValueCollection();

                string url = manager.Uri;
                int indexOfQuery = url.IndexOf('?');
                if (indexOfQuery >= 0)
                {
                    string query = url.Substring(indexOfQuery + 1).ToLowerInvariant();
                    string[] items = query.Split('&');
                    foreach (string parameter in items)
                    {
                        string[] keyValue = parameter.Split('=');
                        if (keyValue.Length == 2)
                            parameters.Add(keyValue[0], keyValue[1]);
                        else
                            parameters.Add(keyValue[0], null);
                    }
                }
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                EnsureParameters();
                return parameters.Keys;
            }
        }

        public bool TryGet<T>(string key, out T value)
        {
            EnsureParameters();
            return parameters.TryGet(key?.ToLowerInvariant(), out value);
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            manager.LocationChanged -= OnLocationChanged;
        }
    }
}
