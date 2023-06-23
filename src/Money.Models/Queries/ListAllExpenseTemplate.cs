using Neptuo.Formatters.Metadata;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    /// <summary>
    /// A query for getting all expense templates.
    /// </summary>
    public class ListAllExpenseTemplate : UserQuery, IQuery<List<ExpenseTemplateModel>>
    {
        [CompositeVersion]
        [CompositeProperty(1, Version = 2)]
        public int Version { get; private set; }

        [CompositeConstructor(Version = 1)]
        public ListAllExpenseTemplate()
            : this(1)
        { }

        [CompositeConstructor(Version = 2)]
        public ListAllExpenseTemplate(int version = 2) 
            => Version = version;

        /// <summary>
        /// Creates a new instance which returns objects in version 2.
        /// </summary>
        public static ListAllExpenseTemplate Version2() 
            => new ListAllExpenseTemplate(2);

        /// <summary>
        /// Creates a new instance which returns objects in version 2.
        /// </summary>
        public static ListAllExpenseTemplate Version3() 
            => new ListAllExpenseTemplate(3);
    }
}
