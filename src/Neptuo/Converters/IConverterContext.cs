using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Converters
{
    /// <summary>
    /// The contract for replacement of source value in the generic <see cref="IConverter{TSource, TTarget}"/>.
    /// The source type parameter can be replace by this interface and <see cref="DefaultConverterRepository"/> will pass it in.
    /// 
    /// It is usefull when composite convertion is required. Eg: Converting list of objects.
    /// </summary>
    public interface IConverterContext<TSource>
    {
        /// <summary>
        /// Gets the original source value.
        /// </summary>
        TSource SourceValue { get; }

        /// <summary>
        /// Gets the repository, where the conversion was invoked.
        /// </summary>
        IConverterRepository Repository { get; }
    }
}
