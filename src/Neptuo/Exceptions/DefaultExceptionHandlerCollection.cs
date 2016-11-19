using Neptuo.Exceptions.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Exceptions
{
    /// <summary>
    /// The default implementation of the <see cref="IExceptionHandlerCollection"/>
    /// </summary>
    public class DefaultExceptionHandlerCollection : IExceptionHandlerCollection
    {
        private readonly List<IExceptionHandler> storage = new List<IExceptionHandler>();

        public IEnumerable<IExceptionHandler> Enumerate()
        {
            return storage;
        }

        public IExceptionHandlerCollection Add(IExceptionHandler handler)
        {
            Ensure.NotNull(handler, "handler");
            storage.Add(handler);
            return this;
        }
    }
}
