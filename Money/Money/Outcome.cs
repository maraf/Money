using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// The model of outcome.
    /// </summary>
    public class Outcome
    {
        /// <summary>
        /// Gets or sets an ID of the outcome.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets a description of the outcome.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets an amount of the outcome.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a date when the outcome occured.
        /// </summary>
        public DateTime When { get; set; }

        /// <summary>
        /// Gets a set of assigned tag ids.
        /// </summary>
        public HashSet<Guid> TagIds { get; private set; }

        public Outcome()
        {
            TagIds = new HashSet<Guid>();
        }
    }
}
