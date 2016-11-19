using Neptuo.Exceptions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Keys
{
    /// <summary>
    /// Keys extensions of <see cref="Ensure.Condition"/>.
    /// </summary>
    public static class _EnsureConditionExtensions
    {
        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> when <paramref name="key"/> is <see cref="IKey.Empty"/> (= true).
        /// </summary>
        /// <param name="condition">The condition helper.</param>
        /// <param name="key">The to test.</param>
        public static void NotEmptyKey(this EnsureConditionHelper condition, IKey key)
        {
            Ensure.NotNull(condition, "condition");
            Ensure.NotNull(key, "key");

            if (key.IsEmpty)
                throw new RequiredNotEmptyKeyException(key.Type);
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> when <paramref name="key"/> has not <paramref name="type"/> as type.
        /// </summary>
        /// <param name="condition">The condition helper.</param>
        /// <param name="key">The to test.</param>
        /// <param name="type">The required key type.</param>
        /// <param name="isNotEmptyRequired">Is <c>true</c> only not empty key is accepted</param>
        public static void NotDifferentKeyType(this EnsureConditionHelper condition, IKey key, string type, bool isNotEmptyRequired = true)
        {
            Ensure.NotNull(condition, "condition");
            Ensure.NotNull(key, "key");
            Ensure.NotNullOrEmpty(type, "type");

            if (isNotEmptyRequired)
                NotEmptyKey(condition, key);

            if (key.Type != type)
                throw new RequiredKeyOfTypeException(key.Type, type);
        }

        [Obsolete]
        public static void NotEmptyKey(this EnsureConditionHelper condition, IKey key, string argumentName)
        {
            NotEmptyKey(condition, key);
        }

        [Obsolete]
        public static void NotDifferentKeyType(this EnsureConditionHelper condition, IKey key, string type, string argumentName)
        {
            NotDifferentKeyType(condition, key, type, false);
        }
    }
}
