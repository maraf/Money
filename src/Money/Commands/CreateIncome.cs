using Neptuo;
using Neptuo.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    /// <summary>
    /// Creates an income.
    /// </summary>
    public class CreateIncome : Command
    {        
        /// <summary>
        /// Gets an amount of the income.
        /// </summary>
        public Price Amount { get; private set; }

        /// <summary>
        /// Gets a description of the income.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a date and time when the income occured.
        /// </summary>
        public DateTime When { get; private set; }

        /// <summary>
        /// Creates a new command for adding an income.
        /// </summary>
        /// <param name="amount">An amount of the income.</param>
        /// <param name="description">A description of the income.</param>
        /// <param name="when">A date and time when the income occured.</param>
        public CreateIncome(Price amount, string description, DateTime when)
        {
            Ensure.NotNull(amount, "amount");
            Ensure.NotNull(description, "description");
            Ensure.NotNull(when, "when");
            Amount = amount;
            Description = description;
            When = when;
        }
    }
}
