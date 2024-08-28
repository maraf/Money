using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class OutcomeEntity : IPriceFixed, IUserEntity
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime When { get; set; }
        public IList<OutcomeCategoryEntity> Categories { get; set; }
        public bool IsFixed { get; set; }

        Price IPriceFixed.Amount => new Price(Amount, Currency);
        DateTime IPriceFixed.When => When;

        public OutcomeEntity()
        { }

        public OutcomeEntity(OutcomeModel model)
        {
            Id = model.Key.AsGuidKey().Guid;
            Description = model.Description;
            Amount = model.Amount.Value;
            Currency = model.Amount.Currency;
            When = model.When;
            IsFixed = model.IsFixed;
            Categories = new List<OutcomeCategoryEntity>();

            foreach (IKey categoryKey in model.CategoryKeys)
            {
                Categories.Add(new OutcomeCategoryEntity()
                {
                    OutcomeId = Id,
                    CategoryId = categoryKey.AsGuidKey().Guid
                });
            }
        }

        private GuidKey GetCategoryKey()
            => GuidKey.Create(Categories.First().CategoryId, KeyFactory.Empty(typeof(Category)).Type);

        private Price GetAmount()
            => new Price(Amount, Currency);

        private GuidKey GetKey()
            => GuidKey.Create(Id, KeyFactory.Empty(typeof(Outcome)).Type);

        public OutcomeModel ToModel()
        {
            string categoryKeyType = KeyFactory.Empty(typeof(Category)).Type;
            return new OutcomeModel(
                GetKey(),
                GetAmount(),
                When,
                Description,
                Categories.Select(c => GuidKey.Create(c.CategoryId, categoryKeyType)).ToList(),
                IsFixed
            );
        }

        public OutcomeOverviewModel ToOverviewModel(int version)
        {
            GuidKey outcomeKey = GetKey();
            GuidKey categoryKey = GetCategoryKey();
            Price amount = GetAmount();
            if (version == 1)
            {
                return new OutcomeOverviewModel(
                    outcomeKey,
                    amount,
                    When,
                    Description,
                    categoryKey
                );
            }
            else if (version == 2)
            {
                return new OutcomeOverviewModel(
                    outcomeKey,
                    amount,
                    When,
                    Description,
                    categoryKey,
                    IsFixed
                );
            }
            else
            {
                throw Ensure.Exception.InvalidOperation($"Invalid version '{version}' of expense overview model.");
            }
        }

        public ExpenseChecklistModel ToExpenseChecklistModel(IKey expenseTemplateKey) => new ExpenseChecklistModel(
            expenseTemplateKey,
            GetKey(),
            GetAmount(),
            When,
            GetCategoryKey(),
            Description,
            IsFixed
        );
    }
}
