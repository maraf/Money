﻿using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when a description of the expense template has changed.
    /// </summary>
    public class ExpenseTemplateDescriptionChanged : UserEvent
    {
        /// <summary>
        /// Gets a new value of the description.
        /// </summary>
        public string Description { get; private set; }

        internal ExpenseTemplateDescriptionChanged(string description)
        {
            Description = description;
        }
    }
}
