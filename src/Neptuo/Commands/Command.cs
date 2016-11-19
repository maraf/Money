using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands
{
    /// <summary>
    /// The base implementation of <see cref="ICommand"/>.
    /// </summary>
    public abstract class Command : ICommand
    {
        /// <summary>
        /// The key of this command.
        /// </summary>
        public IKey Key { get; internal set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        protected Command()
        {
            Key = KeyFactory.Create(GetType());
        }

        /// <summary>
        /// Creates new instance with the <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of this command.</param>
        protected Command(IKey key)
        {
            Ensure.Condition.NotEmptyKey(key, "key");
            Key = key;
        }
    }
}
