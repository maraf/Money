using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// Activator for <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">Type of service to create.</typeparam>
    public interface IFactory<out T>
    {
        /// <summary>
        /// Creates service of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>Service of type <typeparamref name="T"/>.</returns>
        T Create();
    }

    /// <summary>
    /// Activator for <typeparamref name="T"/> with posibility to use parameters for inicialization.
    /// </summary>
    /// <typeparam name="T">Type of service to create.</typeparam>
    /// <typeparam name="TContext">Type of context for activation.</typeparam>
    public interface IFactory<out T, in TContext>
    {
        /// <summary>
        /// Creates service of type <typeparamref name="T"/> with posibility to use <paramref name="context"/> for inicialization.
        /// </summary>
        /// <param name="context">Activation context.</param>
        /// <returns>Service of type <typeparamref name="T"/>.</returns>
        T Create(TContext context);
    }
}
