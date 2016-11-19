using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    /// <summary>
    /// Very basic implementation of <see cref="ICompositeDelegateFactory"/> which simple recalls reflection objects.
    /// </summary>
    public class ReflectionCompositeDelegateFactory : ICompositeDelegateFactory
    {
        public Func<object, object> CreatePropertyGetter(PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetMethod == null)
                return null;

            return instance => propertyInfo.GetValue(instance);
        }

        public Action<object, object> CreatePropertySetter(PropertyInfo propertyInfo)
        {
            if (propertyInfo.SetMethod == null)
                return null;

            return (instance, value) => propertyInfo.SetValue(instance, value);
        }

        public Func<object[], object> CreateConstructorFactory(ConstructorInfo constructorInfo)
        {
            return parameters => constructorInfo.Invoke(parameters);
        }
    }
}
