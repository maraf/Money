using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// Implementation of <see cref="IFactory{T}"/> for types with parameterless constructor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultFactory<T> : IFactory<T>
        where T  : new()
    {
        public T Create()
        {
            return new T();
        }
    }
}
