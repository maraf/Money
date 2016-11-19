using Neptuo.Events;
using Neptuo.Models.Keys;
using Neptuo.Models.Snapshots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// The builder for constructor.
    /// </summary>
    public class ReflectionAggregateRootFactoryBuilder
    {
        private readonly List<Parameter> parameters = new List<Parameter>();

        internal List<Parameter> Parameters
        {
            get { return parameters; }
        }

        /// <summary>
        /// Adds <paramref name="parameter"/> as constant/singleton parameter.
        /// </summary>
        /// <typeparam name="T">Type of the parameter to register as.</typeparam>
        /// <param name="parameter">The instanci of the parameter.</param>
        /// <returns>Self (for fluency).</returns>
        public ReflectionAggregateRootFactoryBuilder Add<T>(T parameter)
        {
            parameters.Add(new Parameter(typeof(T), parameter));
            return this;
        }

        /// <summary>
        /// Adds placeholder for a parameter containing the key of the aggregate root.
        /// </summary>
        /// <returns>Self (for fluency).</returns>
        public ReflectionAggregateRootFactoryBuilder AddKey()
        {
            parameters.Add(new Parameter(Parameter.KeyType));
            return this;
        }

        /// <summary>
        /// Adds placeholder for a parameter containing the stream of events representing the aggregate root's history.
        /// </summary>
        /// <returns>Self (for fluency).</returns>
        public ReflectionAggregateRootFactoryBuilder AddHistory()
        {
            parameters.Add(new Parameter(Parameter.HistoryType));
            return this;
        }

        /// <summary>
        /// Adds placeholder for a parameter containing the latest aggregate snapshot.
        /// </summary>
        /// <returns>Self (for fluency).</returns>
        public ReflectionAggregateRootFactoryBuilder AddSnapshot()
        {
            parameters.Add(new Parameter(Parameter.SnapshotType));
            return this;
        }

        internal class Parameter
        {
            /// <summary>
            /// Gets a type of the key parameter.
            /// </summary>
            public static readonly Type KeyType = typeof(IKey);

            /// <summary>
            /// Gets a type of the history parameter.
            /// </summary>
            public static readonly Type HistoryType = typeof(IEnumerable<IEvent>);

            /// <summary>
            /// Gets a type of the snapshot type.
            /// </summary>
            public static readonly Type SnapshotType = typeof(ISnapshot);

            public Type Type { get; private set; }
            public object Value { get; private set; }
            public bool IsInternalParameter { get; private set; }

            internal Parameter(Type type, object parameter)
            {
                Type = type;
                Value = parameter;
            }

            internal Parameter(Type type)
            {
                Type = type;
                IsInternalParameter = true;
            }
        }
    }
}
