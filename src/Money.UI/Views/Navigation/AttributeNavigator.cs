using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Navigation
{
    /// <summary>
    /// A default implementation of <see cref="INavigator"/>.
    /// </summary>
    internal class AttributeNavigator : INavigator
    {
        private readonly Dictionary<Type, Type> storage = new Dictionary<Type, Type>();
        private readonly Frame frame;

        public AttributeNavigator(Frame frame)
        {
            Ensure.NotNull(frame, "frame");
            this.frame = frame;

            foreach (Type type in GetType().GetTypeInfo().Assembly.GetTypes())
            {
                NavigationParameterAttribute attribute = type.GetTypeInfo().GetCustomAttribute<NavigationParameterAttribute>();
                if (attribute != null)
                    storage[attribute.ParameterType] = type;
            }
        }

        public INavigatorForm Open(object parameter)
        {
            Ensure.NotNull(parameter, "parameter");
            Type parameterType = parameter.GetType();
            Type pageType;
            if (storage.TryGetValue(parameterType, out pageType))
                return new NavigatorForm(frame, pageType, parameter);

            throw Ensure.Exception.InvalidOperation("Missing page for parameter of type '{0}'.", parameterType.FullName);
        }

        private class NavigatorForm : INavigatorForm
        {
            private readonly Frame frame;
            private readonly Type pageType;
            private readonly object parameter;

            public NavigatorForm(Frame frame, Type pageType, object parameter)
            {
                this.frame = frame;
                this.pageType = pageType;
                this.parameter = parameter;
            }

            public void Show()
            {
                frame.Navigate(pageType, parameter);
            }
        }
    }
}
