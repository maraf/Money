using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Converters
{
    /// <summary>
    /// Default implmentation of <see cref="IConverterRepository"/>.
    /// </summary>
    public class DefaultConverterRepository : IConverterRepository
    {
        private readonly object storageLock = new object();
        private readonly Dictionary<Type, Dictionary<Type, IConverter>> storage;
        private readonly OutFuncCollection<ConverterSearchContext, IConverter, bool> onSearchConverter;

        /// <summary>
        /// Cretes new empty instance.
        /// </summary>
        public DefaultConverterRepository()
            : this(new Dictionary<Type, Dictionary<Type, IConverter>>())
        { }

        /// <summary>
        /// Creates instance with default converter registrations.
        /// </summary>
        /// <param name="storage">'First is the source type, second key is the target type' storage.</param>
        public DefaultConverterRepository(Dictionary<Type, Dictionary<Type, IConverter>> storage)
        {
            Ensure.NotNull(storage, "storage");
            this.storage = storage;
            this.onSearchConverter = new OutFuncCollection<ConverterSearchContext, IConverter, bool>();
        }

        public IConverterRepository Add(Type sourceType, Type targetType, IConverter converter)
        {
            Ensure.NotNull(sourceType, "sourceType");
            Ensure.NotNull(targetType, "targetType");
            Ensure.NotNull(converter, "converter");

            Dictionary<Type, IConverter> sourceStorage;
            if (!storage.TryGetValue(sourceType, out sourceStorage))
            {
                lock (storageLock)
                {
                    if (!storage.TryGetValue(sourceType, out sourceStorage))
                        storage[sourceType] = sourceStorage = new Dictionary<Type, IConverter>();
                }
            }

            lock (storageLock)
                sourceStorage[targetType] = converter;

            return this;
        }

        public IConverterRepository AddSearchHandler(OutFunc<ConverterSearchContext, IConverter, bool> searchHandler)
        {
            Ensure.NotNull(searchHandler, "searchHandler");
            lock (storageLock)
                onSearchConverter.Add(searchHandler);

            return this;
        }

        private bool IsConverterContextType(Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IConverterContext<>);
        }

        public bool TryConvert<TSource, TTarget>(TSource sourceValue, out TTarget targetValue)
        {
            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);

            // If target value is assignable from source, no conversion is needed.
            if (targetType.IsAssignableFrom(sourceType))
            {
                targetValue = (TTarget)(object)sourceValue;
                return true;
            }

            // If source value is null, return default value.
            if (sourceValue == null)
            {
                targetValue = default(TTarget);
                return true;
            }

            // Find converter, look in storage or find using search handler.
            IConverter converter = null;
            Dictionary<Type, IConverter> sourceStorage;
            if (!storage.TryGetValue(sourceType, out sourceStorage) || !sourceStorage.TryGetValue(targetType, out converter))
                onSearchConverter.TryExecute(new ConverterSearchContext(sourceType, targetType), out converter);

            // If no converter was found, try context converters.
            if (converter == null && !IsConverterContextType(sourceType))
                return TryConvert<IConverterContext<TSource>, TTarget>(new DefaultConverterContext<TSource>(sourceValue, this), out targetValue);

            // If no converter was found, conversion is not possible.
            if (converter == null)
            {
                targetValue = default(TTarget);
                return false;
            }

            // Try cast to generic converter.
            IConverter<TSource, TTarget> genericConverter = converter as IConverter<TSource, TTarget>;
            if (genericConverter != null)
                return genericConverter.TryConvert(sourceValue, out targetValue);

            // Convert using general converter.
            object targetObject;
            if (converter.TryConvert(sourceType, targetType, sourceValue, out targetObject))
            {
                targetValue = (TTarget)targetObject;
                return true;
            }

            // No other options for conversion.
            targetValue = default(TTarget);
            return false;
        }

        public bool TryConvert(Type sourceType, Type targetType, object sourceValue, out object targetValue)
        {
            Ensure.NotNull(sourceType, "sourceType");
            Ensure.NotNull(targetType, "targetType");

            // If target value is assignable from source, no conversion is needed.
            if (targetType.IsAssignableFrom(sourceType))
            {
                targetValue = sourceValue;
                return true;
            }

            // If source value is null, return default value.
            if (sourceValue == null)
            {
                if (targetType.GetTypeInfo().IsValueType)
                    targetValue = Activator.CreateInstance(targetType);
                else
                    targetValue = null;

                return true;
            }

            // Find converter, look in storage or find using search handler.
            IConverter converter = null;
            Dictionary<Type, IConverter> sourceStorage;
            if (!storage.TryGetValue(sourceType, out sourceStorage) || !sourceStorage.TryGetValue(targetType, out converter))
                onSearchConverter.TryExecute(new ConverterSearchContext(sourceType, targetType), out converter);

            // If no converter was found, try context converters.
            if (converter == null && !IsConverterContextType(sourceType))
            {
                Type sourceContextType = typeof(IConverterContext<>).MakeGenericType(sourceType);

                if (!storage.TryGetValue(sourceContextType, out sourceStorage) || !sourceStorage.TryGetValue(targetType, out converter))
                    onSearchConverter.TryExecute(new ConverterSearchContext(sourceContextType, targetType), out converter);

                // If context converter was found, create instance of context.
                if (converter != null)
                {
                    Type concreteContextType = typeof(DefaultConverterContext<>).MakeGenericType(sourceType);
                    ConstructorInfo concreteContextConstructor = concreteContextType.GetConstructor(new Type[] { sourceType, typeof(IConverterRepository) });
                    if (concreteContextConstructor != null)
                    {
                        sourceValue = concreteContextConstructor.Invoke(new object[] { sourceValue, this });
                        sourceType = sourceContextType;
                    }
                    else
                    {
                        converter = null;
                    }
                }
            }

            // If no converter was found, conversion is not possible.
            if (converter == null)
            {
                targetValue = null;
                return false;
            }

            // Convert using general converter.
            return converter.TryConvert(sourceType, targetType, sourceValue, out targetValue);
        }

        public Func<TSource, TTarget> GetConverter<TSource, TTarget>()
        {
            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);

            // If target value is assignable from source, no conversion is needed.
            if (targetType.IsAssignableFrom(sourceType))
                return sourceValue => (TTarget)(object)sourceValue;

            // Find converter, look in storage or find using search handler.
            IConverter converter = null;
            Dictionary<Type, IConverter> sourceStorage;
            if (!storage.TryGetValue(sourceType, out sourceStorage) || !sourceStorage.TryGetValue(targetType, out converter))
                onSearchConverter.TryExecute(new ConverterSearchContext(sourceType, targetType), out converter);

            // If no converter was found, try context converters.
            if (converter == null)
            {
                Func<IConverterContext<TSource>, TTarget> result = GetConverter<IConverterContext<TSource>, TTarget>();
                return sourceValue => result(new DefaultConverterContext<TSource>(sourceValue, this));
            }

            // If no converter was found, conversion is not possible.
            if (converter == null && !IsConverterContextType(sourceType))
            {
                return sourceValue =>
                {
                    // If source value is null, return default value.
                    if (sourceValue == null)
                        return default(TTarget);

                    return default(TTarget);
                };
            }

            // Try cast to generic converter.
            IConverter<TSource, TTarget> genericConverter = converter as IConverter<TSource, TTarget>;
            if (genericConverter != null)
            {
                return sourceValue =>
                {
                    TTarget targetValue;
                    if (genericConverter.TryConvert(sourceValue, out targetValue))
                        return targetValue;

                    throw Ensure.Exception.NotSupportedConversion(targetType, sourceValue);
                };
            }

            return sourceValue =>
            {
                // Convert using general converter.
                object targetObject;
                if (converter.TryConvert(sourceType, targetType, sourceValue, out targetObject))
                    return (TTarget)targetObject;

                // No other options for conversion.
                return default(TTarget);
            };
        }

        public OutFunc<TSource, TTarget, bool> GetTryConverter<TSource, TTarget>()
        {
            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);

            // If target value is assignable from source, no conversion is needed.
            if (targetType.IsAssignableFrom(sourceType))
            {
                return (TSource sourceValue, out TTarget targetValue) =>
                {
                    targetValue = (TTarget)(object)sourceValue;
                    return true;
                };
            }

            // Find converter, look in storage or find using search handler.
            IConverter converter = null;
            Dictionary<Type, IConverter> sourceStorage;
            if (!storage.TryGetValue(sourceType, out sourceStorage) || !sourceStorage.TryGetValue(targetType, out converter))
                onSearchConverter.TryExecute(new ConverterSearchContext(sourceType, targetType), out converter);

            // If no converter was found, try context converters.
            if (converter == null && !IsConverterContextType(sourceType))
            {
                OutFunc<IConverterContext<TSource>, TTarget, bool> result = GetTryConverter<IConverterContext<TSource>, TTarget>();
                return (TSource sourceValue, out TTarget targetValue) => result(new DefaultConverterContext<TSource>(sourceValue, this), out targetValue);
            }

            // If no converter was found, conversion is not possible.
            if (converter == null)
            {
                return (TSource sourceValue, out TTarget targetValue) =>
                {
                    // If source value is null, return default value.
                    if (sourceValue == null)
                    {
                        targetValue = default(TTarget);
                        return true;
                    }

                    targetValue = default(TTarget);
                    return false;
                };
            }

            // Try cast to generic converter.
            IConverter<TSource, TTarget> genericConverter = converter as IConverter<TSource, TTarget>;
            if (genericConverter != null)
                return genericConverter.TryConvert;

            // Convert using general converter.
            return (TSource sourceValue, out TTarget targetValue) =>
            {
                object targetObject;
                if (converter.TryConvert(sourceType, targetType, sourceValue, out targetObject))
                {
                    targetValue = (TTarget)targetObject;
                    return true;
                }

                // No other options for conversion.
                targetValue = default(TTarget);
                return false;
            };
        }
    }
}
