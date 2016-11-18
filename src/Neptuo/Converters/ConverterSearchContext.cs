using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Converters
{
    /// <summary>
    /// Context for converter search handler.
    /// </summary>
    public class ConverterSearchContext
    {
        /// <summary>
        /// Conversion source type.
        /// </summary>
        public Type SourceType { get; private set; }

        /// <summary>
        /// Conversion target type.
        /// </summary>
        public Type TargetType { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="sourceType">Conversion source type.</param>
        /// <param name="targetType">Conversion target type.</param>
        public ConverterSearchContext(Type sourceType, Type targetType)
        {
            Ensure.NotNull(sourceType, "sourceType");
            Ensure.NotNull(targetType, "targetType");
            SourceType = sourceType;
            TargetType = targetType;
        }
    }
}
