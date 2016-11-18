using Neptuo.Exceptions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Converters
{
    public static class _EnsureExtensions
    {
        public static ArgumentOutOfRangeException NotSupportedConversion(this EnsureExceptionHelper ensure, Type targetType, object sourceValue)
        {
            return ensure.ArgumentOutOfRange(
                "TTarget",
                "Target type '{0}' can't constructed from value '{1}'.",
                targetType.FullName,
                sourceValue
            );
        }
    }
}
