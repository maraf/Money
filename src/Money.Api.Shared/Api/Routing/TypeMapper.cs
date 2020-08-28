using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Api.Routing
{
    public abstract class TypeMapper
    {
        private readonly List<(string Url, Type Type)> storage = new List<(string, Type)>();

        protected void Add<T>(string url)
            => Add(typeof(T), url);

        protected void Add(Type type, string url)
        {
            if (storage.Any(m => m.Url == url))
                throw Ensure.Exception.Argument("url", $"URL '{url}' is already taken.");

            storage.Add((url, type));
        }

        public string FindUrlByType(Type type)
        {
            foreach (var mapping in storage)
            {
                if (mapping.Type == type)
                    return mapping.Url;
            }

            return null;
        }

        public Type FindTypeByUrl(string url)
        {
            foreach (var mapping in storage)
            {
                if (mapping.Url == url)
                    return mapping.Type;
            }

            return null;
        }
    }
}
