using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Converters
{
    /// <summary>
    /// A default implementation of <see cref="IConverterContext{TSource}"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the original source value.</typeparam>
    public class DefaultConverterContext<TSource> : IConverterContext<TSource>
    {
        public TSource SourceValue { get; private set; }
        public IConverterRepository Repository { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="sourceValue">The original source value.</param>
        /// <param name="repository">The repository, where the conversion was invoked.</param>
        public DefaultConverterContext(TSource sourceValue, IConverterRepository repository)
        {
            Ensure.NotNull(repository, "repository");
            SourceValue = sourceValue;
            Repository = repository;
        }
    }
}
