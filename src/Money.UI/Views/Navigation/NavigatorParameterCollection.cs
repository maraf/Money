using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.Navigation
{
    public class NavigatorParameterCollection
    {
        private readonly Dictionary<Type, Type> storage = new Dictionary<Type, Type>();

        public NavigatorParameterCollection()
        {
            foreach (Type type in GetType().GetTypeInfo().Assembly.GetTypes())
            {
                NavigationParameterAttribute attribute = type.GetTypeInfo().GetCustomAttribute<NavigationParameterAttribute>();
                if (attribute != null)
                    storage[attribute.ParameterType] = type;
            }
        }

        public bool TryGetPageType(Type parameterType, out Type pageType)
        {
            Ensure.NotNull(parameterType, "parameterType");
            return storage.TryGetValue(parameterType, out pageType);
        }
    }
}
