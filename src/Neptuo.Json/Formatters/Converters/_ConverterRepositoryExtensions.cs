using Neptuo.Converters;
using Neptuo.Formatters.Internals;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Converters
{
    /// <summary>
    /// Registration extensions of <see cref="IConverterRepository"/>.
    /// </summary>
    public static class _ConverterRepositoryExtensions
    {
        /// <summary>
        /// Adds search handler to the <paramref name="repository"/> for converting primitive types (<see cref="JsonPrimitiveConverter.Supported"/>) from and to <see cref="JToken"/>.
        /// </summary>
        /// <param name="repository">The repository to register handler to.</param>
        /// <returns><paramref name="repository"/>.</returns>
        public static IConverterRepository AddJsonPrimitivesSearchHandler(this IConverterRepository repository)
        {
            Ensure.NotNull(repository, "repository");
            return repository.AddSearchHandler(TryGetJsonPrimitiveConverter);
        }

        private static bool TryGetJsonPrimitiveConverter(ConverterSearchContext context, out IConverter converter)
        {
            bool isSuccess = false;

            if (context.SourceType == typeof(JToken) || context.SourceType == typeof(JValue))
            {
                if (JsonPrimitiveConverter.Supported.Contains(context.TargetType))
                    isSuccess = true;
            }
            else if (context.TargetType == typeof(JToken) || context.TargetType == typeof(JValue))
            {
                if (JsonPrimitiveConverter.Supported.Contains(context.SourceType))
                    isSuccess = true;
            }

            if (isSuccess)
                converter = new JsonPrimitiveConverter();
            else
                converter = null;

            return isSuccess;
        }

        /// <summary>
        /// Adds search handler to the <paramref name="repository"/> for converting enum types from and to <see cref="JToken"/>.
        /// </summary>
        /// <param name="repository">The repository to register handler to.</param>
        /// <param name="converterType">The way how to serialize and deserialize enum values.</param>
        /// <returns><paramref name="repository"/>.</returns>
        public static IConverterRepository AddJsonEnumSearchHandler(this IConverterRepository repository, JsonEnumConverterType converterType = JsonEnumConverterType.UseInderlayingValue)
        {
            Ensure.NotNull(repository, "repository");
            return repository.AddSearchHandler(new TryGetJsonEnumConverter(converterType).TryFind);
        }

        private class TryGetJsonEnumConverter
        {
            private readonly JsonEnumConverterType converterType;

            public TryGetJsonEnumConverter(JsonEnumConverterType converterType)
            {
                Ensure.NotNull(converterType, "converterType");
                this.converterType = converterType;
            }

            public bool TryFind(ConverterSearchContext context, out IConverter converter)
            {
                bool isSuccess = false;

                if (context.SourceType == typeof(JToken))
                {
                    if (context.TargetType.GetTypeInfo().IsEnum)
                        isSuccess = true;
                    else if (context.TargetType.IsNullableType() && context.TargetType.GetGenericArguments()[0].GetTypeInfo().IsEnum)
                        isSuccess = true;
                }
                else if (context.TargetType == typeof(JToken))
                {
                    if (context.SourceType.GetTypeInfo().IsEnum)
                        isSuccess = true;
                    else if (context.SourceType.IsNullableType() && context.SourceType.GetGenericArguments()[0].GetTypeInfo().IsEnum)
                        isSuccess = true;
                }

                if (isSuccess)
                    converter = new JsonEnumConverter(converterType);
                else
                    converter = null;

                return isSuccess;
            }
        }

        /// <summary>
        /// Adds search handler to the <paramref name="repository"/> for converting any object type (not abstract and class) as <see cref="JObject"/>.
        /// </summary>
        /// <param name="repository">The repository to register handler to.</param>
        /// <returns><paramref name="repository"/>.</returns>
        public static IConverterRepository AddJsonObjectSearchHandler(this IConverterRepository repository)
        {
            Ensure.NotNull(repository, "repository");
            return repository.AddSearchHandler(TryGetJsonObjectConverter);
        }

        private static bool TryGetJsonObjectConverter(ConverterSearchContext context, out IConverter converter)
        {
            bool isSuccess = false;
            if (context.SourceType == typeof(JToken))
            {
                if (context.TargetType.GetTypeInfo().IsClass && !context.TargetType.GetTypeInfo().IsAbstract)
                    isSuccess = true;
            }
            else if (context.TargetType == typeof(JToken))
            {
                if (context.SourceType.GetTypeInfo().IsClass && !context.SourceType.GetTypeInfo().IsAbstract)
                    isSuccess = true;
            }

            if (isSuccess)
                converter = new JsonObjectConverter();
            else
                converter = null;

            return isSuccess;
        }
    }
}
