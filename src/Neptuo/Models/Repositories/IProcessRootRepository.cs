using Neptuo.Models.Domains;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Repositories
{
    /// <summary>
    /// The repository for process roots.
    /// </summary>
    /// <typeparam name="T">The type of the process.</typeparam>
    public interface IProcessRootRepository<T> : IRepository<T, IKey>
        where T : ProcessRoot
    {
        /// <summary>
        /// Saves <paramref name="model"/> and uses <paramref name="sourceCommandKey"/> as source command key in
        /// envelopes of unpublished events.
        /// </summary>
        /// <param name="model">The aggregate to save.</param>
        /// <param name="sourceCommandKey">The command key to use as source command in the envelopes of unpublished events.</param>
        void Save(T model, IKey sourceCommandKey);
    }
}
