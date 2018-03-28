using Money.Views.Dialogs;
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
        private readonly Dictionary<Type, Type> pages = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, Type> wizards = new Dictionary<Type, Type>();

        public NavigatorParameterCollection()
        {
            foreach (Type type in GetType().GetTypeInfo().Assembly.GetTypes())
            {
                foreach (NavigationParameterAttribute attribute in type.GetTypeInfo().GetCustomAttributes<NavigationParameterAttribute>())
                {
                    if (typeof(IWizard).IsAssignableFrom(type))
                        wizards[attribute.ParameterType] = type;
                    else
                        pages[attribute.ParameterType] = type;
                }
            }
        }

        public bool TryGetPageType(Type parameterType, out Type pageType)
        {
            Ensure.NotNull(parameterType, "parameterType");
            return pages.TryGetValue(parameterType, out pageType);
        }

        public bool TryGetWizardType(Type parameterType, out Type pageType)
        {
            Ensure.NotNull(parameterType, "parameterType");
            return wizards.TryGetValue(parameterType, out pageType);
        }
    }
}
