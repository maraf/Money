using Neptuo.Events;
using Neptuo.Models.Domains;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Models.Snapshots;
using Neptuo;

namespace Neptuo.Activators
{
    /// <summary>
    /// Reflection based implementation of <see cref="IAggregateRootFactory{T}"/>.
    /// The <typeparamref name="T"/> must have constructor with <see cref="IKey"/> and <see cref="IEnumerable{IEvent}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the aggregate root.</typeparam>
    public class ReflectionAggregateRootFactory<T> : IAggregateRootFactory<T>
        where T : AggregateRoot
    {
        private readonly ConstructorInfo historyConstructorInfo;
        private readonly List<ReflectionAggregateRootFactoryBuilder.Parameter> historyParameters;
        private readonly ConstructorInfo snapshotConstructorInfo;
        private readonly List<ReflectionAggregateRootFactoryBuilder.Parameter> snapshotParameters;

        /// <summary>
        /// Creates new instance. If <typeparamref name="T"/> supports snapshots, it will be supported in the factory.
        /// </summary>
        public ReflectionAggregateRootFactory()
            : this(new ReflectionAggregateRootFactoryBuilder().AddKey().AddHistory())
        {
            snapshotConstructorInfo = TryCreateSnaphotConstructor();
            if (snapshotConstructorInfo != null)
                snapshotParameters = new ReflectionAggregateRootFactoryBuilder().AddKey().AddSnapshot().AddHistory().Parameters;
        }

        /// <summary>
        /// Creates to find default snapshot constructor.
        /// </summary>
        /// <returns>Constructor info or <c>null</c>.</returns>
        private static ConstructorInfo TryCreateSnaphotConstructor()
        {
            Type[] snapshotConstructorParameters = new Type[]
            {
                ReflectionAggregateRootFactoryBuilder.Parameter.KeyType,
                ReflectionAggregateRootFactoryBuilder.Parameter.SnapshotType,
                ReflectionAggregateRootFactoryBuilder.Parameter.HistoryType
            };

            ConstructorInfo constructorInfo = typeof(T).GetConstructor(snapshotConstructorParameters);
            return constructorInfo;
        }

        /// <summary>
        /// Creates new instance with explicitly defined parameters and no support for snaphot.
        /// </summary>
        /// <param name="builder">The constructor parameters builder.</param>
        public ReflectionAggregateRootFactory(ReflectionAggregateRootFactoryBuilder builder)
        {
            Ensure.NotNull(builder, "builder");
            historyParameters = builder.Parameters;

            List<Type> internalParameters = new List<Type>()
            {
                ReflectionAggregateRootFactoryBuilder.Parameter.KeyType,
                ReflectionAggregateRootFactoryBuilder.Parameter.HistoryType
            };

            historyConstructorInfo = CreateConstructorInfo(builder.Parameters, internalParameters);
        }

        /// <summary>
        /// Creates new instance with explicitly defined parameters for standart/history constructor and for snapshot constructor.
        /// </summary>
        /// <param name="historyBuilder">The standart/history constructor parameters builder.</param>
        /// <param name="snapshotBuilder">The snapshot constructor parameters builder.</param>
        public ReflectionAggregateRootFactory(ReflectionAggregateRootFactoryBuilder historyBuilder, ReflectionAggregateRootFactoryBuilder snapshotBuilder)
            : this(historyBuilder)
        {
            Ensure.NotNull(snapshotBuilder, "snapshotBuilder");
            snapshotParameters = snapshotBuilder.Parameters;

            List<Type> internalParameters = new List<Type>()
            {
                ReflectionAggregateRootFactoryBuilder.Parameter.KeyType,
                ReflectionAggregateRootFactoryBuilder.Parameter.SnapshotType,
                ReflectionAggregateRootFactoryBuilder.Parameter.HistoryType
            };

            snapshotConstructorInfo = CreateConstructorInfo(snapshotBuilder.Parameters, internalParameters);
        }

        private ConstructorInfo CreateConstructorInfo(List<ReflectionAggregateRootFactoryBuilder.Parameter> parameters, List<Type> internalParameters)
        {
            foreach (ReflectionAggregateRootFactoryBuilder.Parameter parameter in parameters)
            {
                if (parameter.IsInternalParameter)
                    internalParameters.Remove(parameter.Type);
            }

            if (internalParameters.Count > 0)
                throw Ensure.Exception.InvalidOperation("Missing these required parameters of these types {0}.", String.Join(", ", internalParameters.Select(t => "'" + t.FullName + "'")));

            Type[] parameterTypes = parameters.Select(p => p.Type).ToArray();

            ConstructorInfo constructorInfo = typeof(T).GetConstructor(parameterTypes);
            if (constructorInfo == null)
                throw Ensure.Exception.InvalidOperation("Missing the constructor in '{0}' with parameters {1}.", typeof(T).FullName, String.Join(", ", parameterTypes.Select(t => "'" + t.FullName + "'")));

            return constructorInfo;
        }

        public T Create(IKey aggregateKey, IEnumerable<object> events)
        {
            Ensure.Condition.NotEmptyKey(aggregateKey);
            Ensure.NotNull(events, "events");

            object[] parameters = new object[historyParameters.Count];
            for (int i = 0; i < historyParameters.Count; i++)
            {
                ReflectionAggregateRootFactoryBuilder.Parameter parameter = this.historyParameters[i];
                if (parameter.IsInternalParameter)
                {
                    if (parameter.Type == ReflectionAggregateRootFactoryBuilder.Parameter.KeyType)
                        parameters[i] = aggregateKey;
                    else if (parameter.Type == ReflectionAggregateRootFactoryBuilder.Parameter.HistoryType)
                        parameters[i] = events;
                    else
                        throw Ensure.Exception.NotSupported("Not supported internal parameter of type '{0}'.", parameter.Type.FullName);
                }
                else
                {
                    parameters[i] = parameter.Value;
                }
            }

            return (T)historyConstructorInfo.Invoke(parameters);
        }

        public T Create(IKey aggregateKey, ISnapshot snapshot, IEnumerable<object> events)
        {
            Ensure.Condition.NotEmptyKey(aggregateKey);
            Ensure.NotNull(snapshot, "snapshot");
            Ensure.NotNull(events, "events");

            if (snapshotConstructorInfo == null)
                throw new SnapshotConstructorNotSupportedException(typeof(T));

            object[] parameters = new object[snapshotParameters.Count];
            for (int i = 0; i < snapshotParameters.Count; i++)
            {
                ReflectionAggregateRootFactoryBuilder.Parameter parameter = this.snapshotParameters[i];
                if (parameter.IsInternalParameter)
                {
                    if (parameter.Type == ReflectionAggregateRootFactoryBuilder.Parameter.KeyType)
                        parameters[i] = aggregateKey;
                    else if (parameter.Type == ReflectionAggregateRootFactoryBuilder.Parameter.SnapshotType)
                        parameters[i] = snapshot;
                    else if (parameter.Type == ReflectionAggregateRootFactoryBuilder.Parameter.HistoryType)
                        parameters[i] = events;
                    else
                        throw Ensure.Exception.NotSupported("Not supported internal parameter of type '{0}'.", parameter.Type.FullName);
                }
                else
                {
                    parameters[i] = parameter.Value;
                }
            }

            return (T)snapshotConstructorInfo.Invoke(parameters);
        }
    }
}
