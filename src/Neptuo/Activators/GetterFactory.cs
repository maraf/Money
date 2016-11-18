using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// Implementation of <see cref="IFactory{T}"/> that for every call to create instance calls passed delegate.
    /// </summary>
    public class GetterFactory<T> : IFactory<T>
    {
        private readonly Func<T> getter;

        /// <summary>
        /// Creates new instance that uses <paramref name="getter"/> to providing instances.
        /// </summary>
        /// <param name="getter">Instances provider delegate.</param>
        public GetterFactory(Func<T> getter)
        {
            Ensure.NotNull(getter, "getter");
            this.getter = getter;
        }

        public T Create()
        {
            return getter();
        }
    }

    /// <summary>
    /// Implementation of <see cref="IFactory{T, TContext}"/> that for every call to create instance calls passed delegate.
    /// </summary>
    public class GetterFactory<T, TContext> : IFactory<T, TContext>
    {
        private readonly Func<TContext, T> getter;

        /// <summary>
        /// Creates new instance that uses <paramref name="getter"/> to providing instances.
        /// </summary>
        /// <param name="getter">Instances provider delegate.</param>
        public GetterFactory(Func<TContext, T> getter)
        {
            Ensure.NotNull(getter, "getter");
            this.getter = getter;
        }

        public T Create(TContext context)
        {
            return getter(context);
        }
    }
}
