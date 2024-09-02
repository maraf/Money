using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Formatters.Metadata;

namespace Money.Events
{
    /// <summary>
    /// An event raised when the expense template is deleted.
    /// </summary>
    public class ExpenseTemplateDeleted : UserEvent
    {
        [CompositeVersion]
        public int CompositeVersion { get; set; }

        /// <summary>
        /// Gets a date when the template was deleted.
        /// </summary>
        public DateTime DeletedAt { get; private set; }

        [CompositeConstructor(Version = 1)]
        internal ExpenseTemplateDeleted()
        {
            CompositeVersion = 1;
        }

        [CompositeConstructor(Version = 2)]
        internal ExpenseTemplateDeleted(DateTime deletedAt)
            : this()
        {
            DeletedAt = deletedAt;
            CompositeVersion = 2;
        }
    }
}
