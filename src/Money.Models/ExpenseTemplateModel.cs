using Microsoft.VisualBasic;
using Neptuo;
using Neptuo.Formatters.Metadata;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// A model of single expense template.
    /// </summary>
    public class ExpenseTemplateModel : ICloneable<ExpenseTemplateModel>
    {
        [CompositeVersion]
        public int Version { get; private set; }

        /// <summary>
        /// Gets a key of the expense template.
        /// </summary>
        [CompositeProperty(1)]
        [CompositeProperty(1, Version = 2)]
        [CompositeProperty(1, Version = 3)]
        public IKey Key { get; private set; }

        /// <summary>
        /// Gets an amount of the expense template.
        /// </summary>
        [CompositeProperty(2)]
        [CompositeProperty(2, Version = 2)]
        [CompositeProperty(2, Version = 3)]
        public Price Amount { get; set; }

        /// <summary>
        /// Gets a description of the expense template.
        /// </summary>
        [CompositeProperty(3)]
        [CompositeProperty(3, Version = 2)]
        [CompositeProperty(3, Version = 3)]
        public string Description { get; set; }

        /// <summary>
        /// Gets a date when the expense template ocured.
        /// </summary>
        [CompositeProperty(4)]
        [CompositeProperty(4, Version = 2)]
        [CompositeProperty(4, Version = 3)]
        public IKey CategoryKey { get; set; }

        /// <summary>
        /// Gets whether the template should create fixed expenses.
        /// </summary>
        [CompositeProperty(5, Version = 2)]
        [CompositeProperty(5, Version = 3)]
        public bool IsFixed { get; set; }

        /// <summary>
        /// Gets period of recurrence of the template.
        /// </summary>
        [CompositeProperty(6, Version = 3)]
        public RecurrencePeriod? Period { get; set; }

        /// <summary>
        /// Gets day in period of recurrence of the template for <see cref="RecurrencePeriod.Monthly"/>.
        /// </summary>
        [CompositeProperty(7, Version = 3)]
        public int? DayInPeriod { get; set; }

        /// <summary>
        /// Gets due date of the template for <see cref="RecurrencePeriod.Single"/>.
        /// </summary>
        [CompositeProperty(8, Version = 3)]
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="key">A key of the expense template.</param>
        /// <param name="amount">An amount of the expense template.</param>
        /// <param name="description">A description of the expense template.</param> 
        /// <param name="categoryKey">A category of the expense template.</param>
        [CompositeConstructor]
        public ExpenseTemplateModel(IKey key, Price amount, string description, IKey categoryKey)
        {
            Ensure.Condition.NotEmptyKey(key);
            Ensure.NotNull(categoryKey, "categoryKey");
            Key = key;
            Amount = amount;
            CategoryKey = categoryKey;
            Description = description;
            Version = 1;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="key">A key of the expense template.</param>
        /// <param name="amount">An amount of the expense template.</param>
        /// <param name="description">A description of the expense template.</param> 
        /// <param name="categoryKey">A category of the expense template.</param>
        /// <param name="isFixed">Whether the template should create fixed expenses.</param>
        [CompositeConstructor(Version = 2)]
        public ExpenseTemplateModel(IKey key, Price amount, string description, IKey categoryKey, bool isFixed)
            : this(key, amount, description, categoryKey)
        {
            IsFixed = isFixed;
            Version = 2;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="key">A key of the expense template.</param>
        /// <param name="amount">An amount of the expense template.</param>
        /// <param name="description">A description of the expense template.</param> 
        /// <param name="categoryKey">A category of the expense template.</param>
        /// <param name="isFixed">Whether the template should create fixed expenses.</param>
        [CompositeConstructor(Version = 3)]
        public ExpenseTemplateModel(IKey key, Price amount, string description, IKey categoryKey, bool isFixed, RecurrencePeriod? period, int? dayInPeriod, DateTime? dueDate)
            : this(key, amount, description, categoryKey, isFixed)
        {
            Period = period;
            DayInPeriod = dayInPeriod;
            DueDate = dueDate;
            Version = 3;
        }

        public ExpenseTemplateModel Clone() => Version switch
        {
            1 => new ExpenseTemplateModel(Key, Amount, Description, CategoryKey),
            2 => new ExpenseTemplateModel(Key, Amount, Description, CategoryKey, IsFixed),
            3 => new ExpenseTemplateModel(Key, Amount, Description, CategoryKey, IsFixed, Period, DayInPeriod, DueDate),
            _ => throw new NotSupportedException($"Version '{Version}' is not supported when cloning ExpenseTemplateModel")
        };
    }
}
