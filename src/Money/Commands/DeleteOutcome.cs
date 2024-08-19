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
    public class DeleteOutcome : Command, IAggregateRootCommand
    {
        /// <summary>
        /// Gets a key of the outcome to delete.
        /// </summary>
        public IKey OutcomeKey { get; private set; }

        IKey IAggregateRootCommand.AggregateKey => OutcomeKey;

        /// <summary>
        /// Deletes an outcome with <paramref name="outcomeKey"/>.
        /// </summary>
        /// <param name="outcomeKey">A key of the outcome to delete.</param>
        public DeleteOutcome(IKey outcomeKey)
        {
            Ensure.Condition.NotEmptyKey(outcomeKey);
            OutcomeKey = outcomeKey;
        }
    }
}
