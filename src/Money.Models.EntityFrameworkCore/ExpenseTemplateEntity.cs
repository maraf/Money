using Money.Events;
using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class ExpenseTemplateEntity : IUserEntity
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
        public Guid? CategoryId { get; set; }
        public bool IsFixed { get; set; }
        public RecurrencePeriod? Period { get; set; }
        public int? EveryXPeriods { get; set; }
        public int? MonthInPeriod { get; set; }
        public int? DayInPeriod { get; set; }
        public DateTime? DueDate { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ExpenseTemplateEntity()
        { }

        public ExpenseTemplateEntity(ExpenseTemplateCreated payload)
        {
            Id = payload.AggregateKey.AsGuidKey().Guid;
            Description = payload.Description;

            if (payload.Amount != null)
            {
                Amount = payload.Amount.Value;
                Currency = payload.Amount.Currency;
            }

            if (!payload.CategoryKey.IsEmpty)
                CategoryId = payload.CategoryKey.AsGuidKey().Guid;

            IsFixed = payload.IsFixed;

            if (payload.Version >= 3)
                CreatedAt = payload.CreatedAt;
        }

        public ExpenseTemplateModel ToModel(int version) => version switch
        {
            1 => new ExpenseTemplateModel(GetKey(), GetAmount(), Description, GetCategoryKey()),
            2 => new ExpenseTemplateModel(GetKey(), GetAmount(), Description, GetCategoryKey(), IsFixed),
            3 => new ExpenseTemplateModel(GetKey(), GetAmount(), Description, GetCategoryKey(), IsFixed, Period, DayInPeriod, DueDate),
            4 => new ExpenseTemplateModel(GetKey(), GetAmount(), Description, GetCategoryKey(), IsFixed, Period, EveryXPeriods, MonthInPeriod, DayInPeriod, DueDate),
            _ => throw new NotSupportedException($"Version '{version}' is not supported when mapping ExpenseTemplateEntity to ExpenseTemplateModel")
        };

        public ExpenseChecklistModel ToExpenseChecklistModel(DateTime when) => new ExpenseChecklistModel(
            GetKey(),
            KeyFactory.Empty(typeof(Outcome)),
            GetAmount(),
            when,
            GetCategoryKey(),
            Description,
            IsFixed
        );

        public GuidKey GetKey()
            => GuidKey.Create(Id, KeyFactory.Empty(typeof(ExpenseTemplate)).Type);

        private GuidKey GetCategoryKey() => CategoryId != null
            ? GuidKey.Create(CategoryId.Value, KeyFactory.Empty(typeof(Category)).Type)
            : GuidKey.Empty(KeyFactory.Empty(typeof(Category)).Type);

        private Price GetAmount()
            => Amount != null ? new Price(Amount.Value, Currency) : null;
    }
}
