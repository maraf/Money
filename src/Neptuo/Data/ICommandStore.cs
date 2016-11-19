using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data
{
    /// <summary>
    /// The underlying store for commands.
    /// It is responsible for storing serialized commands.
    /// </summary>
    public interface ICommandStore
    {
        /// <summary>
        /// Saves <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to save.</param>
        void Save(CommandModel command);

        /// <summary>
        /// Saves <paramref name="commands"/>.
        /// </summary>
        /// <param name="commands">The commands to save.</param>
        void Save(IEnumerable<CommandModel> commands);
    }
}
