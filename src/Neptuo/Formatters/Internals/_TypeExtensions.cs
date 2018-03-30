using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Internals
{
    public static class _TypeExtensions
    {
        /// <summary>
        /// Returns <c>true</c> if <paramref name="type"/> is <see cref="Nullable{T}"/>.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns><c>true</c> if <paramref name="type"/> is <see cref="Nullable{T}"/>; <c>false</c> otherwise.</returns>
        public static bool IsNullableType(this Type type)
        {
            Ensure.NotNull(type, "type");
            if (type.GetTypeInfo().IsGenericType)
                return type.GetGenericTypeDefinition() == typeof(Nullable<>);

            return false;
        }
    }
}
