using Neptuo.Exceptions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    public static class _EnsureExceptionExtensions
    {
        public static ArgumentOutOfRangeException NotStringKey(this EnsureExceptionHelper exception, Type keyType, string argumentName)
        {
            return Ensure.Exception.ArgumentOutOfRange(
                argumentName,
                "The entity event store currently supportes only string keys, not '{0}'.",
                keyType.Name
            );
        }

        public static ArgumentOutOfRangeException NotGuidKey(this EnsureExceptionHelper exception, Type keyType, string argumentName)
        {
            return Ensure.Exception.ArgumentOutOfRange(
                argumentName,
                "The entity event store currently supportes only Guid keys, not '{0}'.",
                keyType.Name
            );
        }
    }
}
