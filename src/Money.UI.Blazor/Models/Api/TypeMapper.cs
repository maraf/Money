using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Api
{
    public abstract class TypeMapper
    {
        private readonly List<(string Url, Type Type)> urlToType = new List<(string, Type)>();

        protected void Add(string url, Type type)
        {
            if (urlToType.Any(m => m.Url == url))
                throw Ensure.Exception.Argument("url", $"URL '{url}' is already taken.");

            urlToType.Add((url, type));
        }

        public string FindUrlByType(Type type)
        {
            foreach (var mapping in urlToType)
            {
                if (mapping.Type == type)
                    return mapping.Url;
            }

            return null;
        }

        public Type FindTypeByUrl(string url)
        {
            foreach (var mapping in urlToType)
            {
                if (mapping.Url == url)
                    return mapping.Type;
            }

            return null;
        }
    }
}
