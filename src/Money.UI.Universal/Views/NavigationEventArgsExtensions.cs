using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Money.Views
{
    public static class NavigationEventArgsExtensions
    {
        public static T FindParameter<T>(this NavigationEventArgs e)
            where T : class
        {
            Ensure.NotNull(e, "e");
            return e.Parameter as T;
        }

        public static T GetParameter<T>(this NavigationEventArgs e)
            where T : class
        {
            Ensure.NotNull(e, "e");
            return FindParameter<T>(e) ?? throw new InvalidOperationException($"Missing parameter of type '{typeof(T).FullName}'.");
        }

        public static T GetParameterOrDefault<T>(this NavigationEventArgs e, Func<T> defaultValueGetter)
            where T : class
        {
            Ensure.NotNull(e, "e");
            Ensure.NotNull(defaultValueGetter, "defaultValueGetter");
            return FindParameter<T>(e) ?? defaultValueGetter();
        }

        public static T GetParameterOrDefault<T>(this NavigationEventArgs e)
            where T : class, new()
        {
            Ensure.NotNull(e, "e");
            return FindParameter<T>(e) ?? new T();
        }
    }
}
