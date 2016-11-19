using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data
{
    /// <summary>
    /// Entity for persisting command.
    /// </summary>
    public class CommandModel
    {
        /// <summary>
        /// Gets or sets the key of the command.
        /// </summary>
        public IKey CommandKey { get; set; }

        /// <summary>
        /// Gets or sets a serialized command body.
        /// </summary>
        public string Payload { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time when the command has raised.
        /// </summary>
        public DateTime RaisedAt { get; set; }

        /// <summary>
        /// Creates new empty instance.
        /// </summary>
        public CommandModel()
        {
            RaisedAt = DateTime.Now;
        }

        /// <summary>
        /// Creates new instance and fills values.
        /// </summary>
        /// <param name="commandKey">The key of the command.</param>
        /// <param name="payload">A serialized command body.</param>
        public CommandModel(IKey commandKey, string payload)
            : this()
        {
            Ensure.Condition.NotEmptyKey(commandKey);
            CommandKey = commandKey;
            Payload = payload;
        }
    }
}
