using Money.Services.Models;
using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Data
{
    internal class OutcomeEntity
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime When { get; set; }
        public IList<OutcomeCategoryEntity> Categories { get; set; }

        public OutcomeEntity()
        { }

        public OutcomeEntity(OutcomeModel model)
        {
            Id = model.Key.AsGuidKey().Guid;
            Description = model.Description;
            Amount = model.Amount.Value;
            Currency = model.Amount.Currency;
            When = model.When;
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

        public OutcomeModel ToModel()
        {
            string categoryKeyType = KeyFactory.Empty(typeof(Category)).Type;
            return new OutcomeModel(
                GuidKey.Create(Id, KeyFactory.Empty(typeof(Outcome)).Type),
                new Price(Amount, Currency),
                When,
                Description,
                Categories.Select(c => GuidKey.Create(c.CategoryId, categoryKeyType)).ToList()
            );
        }
    }
}
