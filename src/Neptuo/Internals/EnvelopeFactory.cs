using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Internals
{
    /// <summary>
    /// The factory method over reflection for creating generic <see cref="Envelope{T}"/> 
    /// when generic type is not known at compile type.
    /// </summary>
    internal static class EnvelopeFactory
    {
        public static Type GetType(Type payloadType)
        {
            return typeof(Envelope<>).MakeGenericType(payloadType);
        }

        public static Envelope Create(object payload, Type payloadType)
        {
            MethodInfo envelopeCreateMethod = typeof(Envelope)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name == "Create" && m.IsGenericMethod)
                .MakeGenericMethod(payloadType);

            Envelope envelope = (Envelope)envelopeCreateMethod.Invoke(null, new object[] { payload });
            return envelope;
        }
    }
}
