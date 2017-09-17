using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.Navigation
{
    /// <summary>
    /// Associates parameter type with page.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NavigationParameterAttribute : Attribute
    {
        /// <summary>
        /// Gets a type of the expected parameter.
        /// </summary>
        public Type ParameterType { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="parameterType">A type of the expected parameter.</param>
        public NavigationParameterAttribute(Type parameterType)
        {
            Ensure.NotNull(parameterType, "parameterType");
            ParameterType = parameterType;
        }
    }
}
