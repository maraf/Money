using Neptuo.Converters;
using Neptuo.Models.Keys;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Converters
{
    /// <summary>
    /// The extensions for registering converters between keys and JSON.
    /// </summary>
    public static class _ConverterRepositoryExtensions2
    {
        /// <summary>
        /// Adds converters from/to <see cref="GuidKey"/>, <see cref="StringKey"/> and <see cref="IKey"/> (of type <see cref="GuidKey"/> or <see cref="StringKey"/>) and <see cref="JToken"/>.
        /// </summary>
        /// <param name="repository">The repository to add converters to.</param>
        /// <returns>Self (for fluency).</returns>
        public static IConverterRepository AddJsonKey(this IConverterRepository repository)
        {
            Ensure.NotNull(repository, "repository");

            GuidKeyToJTokenConverter guidConverter = new GuidKeyToJTokenConverter();
            StringKeyToJTokenConverter stringConverter = new StringKeyToJTokenConverter();
            KeyToJTokenConverter keyConverter = new KeyToJTokenConverter();

            return repository
                .Add<GuidKey, JToken>(guidConverter)
                .Add<JToken, GuidKey>(guidConverter)
                .Add(typeof(List<GuidKey>), typeof(JToken), guidConverter)
                .Add(typeof(JToken), typeof(IEnumerable<GuidKey>), guidConverter)
                .Add<StringKey, JToken>(stringConverter)
                .Add<JToken, StringKey>(stringConverter)
                .Add(typeof(List<StringKey>), typeof(JToken), stringConverter)
                .Add(typeof(JToken), typeof(IEnumerable<StringKey>), stringConverter)
                .Add<IKey, JToken>(keyConverter)
                .Add<JToken, IKey>(keyConverter)
                .Add(typeof(List<IKey>), typeof(JToken), keyConverter)
                .Add(typeof(JToken), typeof(IEnumerable<IKey>), keyConverter);
        }

        /// <summary>
        /// Adds converter to the <paramref name="repository"/> for converting <see cref="TimeSpan"/> to/from <see cref="JToken"/>.
        /// </summary>
        /// <param name="repository">The repository to add converters to.</param>
        /// <returns>Self (for fluency).</returns>
        public static IConverterRepository AddJsonTimeSpan(this IConverterRepository repository)
        {
            Ensure.NotNull(repository, "repository");

            TimeSpanToJsonConverter converter = new TimeSpanToJsonConverter();

            return repository
                .Add<TimeSpan, JToken>(converter)
                .Add<JToken, TimeSpan>(converter)
                .Add<JValue, TimeSpan>(converter)
                .Add<string, TimeSpan>(TimeSpan.TryParse);
        }
    }
}
