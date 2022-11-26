using Money.Events;
using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        }

        public ExpenseTemplateModel ToModel(int version) => version == 1
            ? new ExpenseTemplateModel(GetKey(), GetAmount(), Description, GetCategoryKey())
            : new ExpenseTemplateModel(GetKey(), GetAmount(), Description, GetCategoryKey(), IsFixed);

        private GuidKey GetKey()
        {
            return GuidKey.Create(Id, KeyFactory.Empty(typeof(ExpenseTemplate)).Type);
        }

        private GuidKey GetCategoryKey() => CategoryId != null
            ? GuidKey.Create(CategoryId.Value, KeyFactory.Empty(typeof(Category)).Type)
            : GuidKey.Empty(KeyFactory.Empty(typeof(Category)).Type);

        private Price GetAmount()
            => Amount != null ? new Price(Amount.Value, Currency) : null;
    }
}
