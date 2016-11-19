using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    /// <summary>
    /// The factory for delegates for fast access.
    /// </summary>
    public interface ICompositeDelegateFactory
    {
        /// <summary>
        /// Creates delegate for accessing value of <paramref name="propertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">The property to wrap to the delegate.</param>
        /// <returns>The delegate that takes instance of the type and returns <paramref name="propertyInfo"/> value.</returns>
        Func<object, object> CreatePropertyGetter(PropertyInfo propertyInfo);

        /// <summary>
        /// Creates delegate for setting value of <paramref name="propertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">The property to wrap to the delegate.</param>
        /// <returns>The delegate that takes instance of the type and new <paramref name="propertyInfo"/> value and should set it to the instance.</returns>
        Action<object, object> CreatePropertySetter(PropertyInfo propertyInfo);

        /// <summary>
        /// Creates delegate for creating instance using <paramref name="constructorInfo"/>.
        /// </summary>
        /// <param name="constructorInfo">The constructor to wrap to the delegate.</param>
        /// <returns>The delegate that takes array of parameters and returns instance of the type.</returns>
        Func<object[], object> CreateConstructorFactory(ConstructorInfo constructorInfo);
    }
}
