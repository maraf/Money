using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    /// <summary>
    /// The factory used for creating instance of <see cref="IKey"/>.
    /// </summary>
    public static class KeyFactory
    {
        private static Func<Type, IKey> keyFactory = targetType => GuidKey.Create(Guid.NewGuid(), targetType.AssemblyQualifiedName);
        private static Func<Type, IKey> emptyFactory = targetType => GuidKey.Empty(targetType.AssemblyQualifiedName);

        /// <summary>
        /// Sets <paramref name="keyFactory"/> to be used for generating new keys and <paramref name="emptyFactory"/> for generating empty keys.
        /// </summary>
        /// <param name="keyFactory">The key generator function.</param>
        /// <param name="emptyFactory">The empty key generator function.</param>
        public static void Set(Func<Type, IKey> keyFactory, Func<Type, IKey> emptyFactory)
        {
            Ensure.NotNull(keyFactory, "keyFactory");
            Ensure.NotNull(emptyFactory, "emptyFactory");
            KeyFactory.keyFactory = keyFactory;
            KeyFactory.emptyFactory = emptyFactory;
        }

        /// <summary>
        /// Sets key factory to use <see cref="GuidKey"/> and type fullname with assembly name (without version and public key).
        /// </summary>
        public static void SetGuidKeyWithTypeFullNameAndAssembly()
        {
            keyFactory = targetType => GuidKey.Create(Guid.NewGuid(), targetType.FullName + ", " + targetType.GetTypeInfo().Assembly.GetName().Name);
            emptyFactory = targetType => GuidKey.Empty(targetType.FullName + ", " + targetType.GetTypeInfo().Assembly.GetName().Name);
        }

        /// <summary>
        /// Creates new instance of a key implementing <see cref="IKey"/> for the <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The type for which key is generated.</param>
        /// <returns>Newly generated key for the <paramref name="targetType"/>.</returns>
        public static IKey Create(Type targetType)
        {
            return keyFactory(targetType);
        }

        /// <summary>
        /// Creates new empty key isntance.
        /// </summary>
        /// <param name="targetType">The type for which key is generated.</param>
        /// <returns>New empty key isntance.</returns>
        public static IKey Empty(Type targetType)
        {
            return emptyFactory(targetType);
        }
    }
}
