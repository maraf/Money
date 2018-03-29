using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    /// <summary>
    /// Deletes an outcome with <see cref="OutcomeKey"/>.
    /// </summary>
    public class DeleteOutcome : Command
    {
        /// <summary>
        /// Gets a key of the outcome to delete.
        /// </summary>
        public IKey OutcomeKey { get; private set; }

        public DeleteOutcome(IKey outcomeKey)
        {
            Ensure.Condition.NotEmptyKey(outcomeKey);
            OutcomeKey = outcomeKey;
        }
    }
}
