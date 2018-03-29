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
    /// Changes a <see cref="When"/> of the outcome with <see cref="OutcomeKey"/>.
    /// </summary>
    public class ChangeOutcomeWhen : Command
    {
        /// <summary>
        /// Gets a key of the outcome to modify.
        /// </summary>
        public IKey OutcomeKey { get; private set; }

        /// <summary>
        /// Gets a date when the outcome occured.
        /// </summary>
        public DateTime When { get; private set; }

        public ChangeOutcomeWhen(IKey outcomeKey, DateTime when)
        {
            Ensure.Condition.NotEmptyKey(outcomeKey);
            Ensure.NotNull(when, "when");
            OutcomeKey = outcomeKey;
            When = when;
        }
    }
}
