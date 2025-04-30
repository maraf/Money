using Money.Models.Sorting;
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
        [CompositeProperty(1, Version = 3)]
        [CompositeProperty(1, Version = 4)]
        public int Version { get; private set; }

        /// <summary>
        /// Intentionally not serialized as the sorting is implemented in clients middleware.
        /// </summary>
        public SortDescriptor<ExpenseTemplateSortType> SortDescriptor { get; private set; }

        [CompositeConstructor(Version = 1)]
        public ListAllExpenseTemplate()
            : this(1)
        { }

        [CompositeConstructor(Version = 2)]
        [CompositeConstructor(Version = 3)]
        [CompositeConstructor(Version = 4)]
        public ListAllExpenseTemplate(int version) 
            => Version = version;

        /// <summary>
        /// Creates a new instance which returns objects in version 2.
        /// </summary>
        public static ListAllExpenseTemplate Version2() 
            => new ListAllExpenseTemplate(2);

        /// <summary>
        /// Creates a new instance which returns objects in version 3.
        /// </summary>
        public static ListAllExpenseTemplate Version3(SortDescriptor<ExpenseTemplateSortType> sortDescriptor = null) 
            => new ListAllExpenseTemplate(3) { SortDescriptor = sortDescriptor };

        /// <summary>
        /// Creates a new instance which returns objects in version 3.
        /// </summary>
        public static ListAllExpenseTemplate Version4(SortDescriptor<ExpenseTemplateSortType> sortDescriptor = null) 
            => new ListAllExpenseTemplate(4) { SortDescriptor = sortDescriptor };
    }
}
