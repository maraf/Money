using Money.Events;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class IncomeEntity : IPriceFixed, IUserEntity
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime When { get; set; }

        Price IPriceFixed.Amount => new Price(Amount, Currency);
        DateTime IPriceFixed.When => When;

        public IncomeEntity(IncomeCreated payload)
        {
            Id = payload.AggregateKey.AsGuidKey().Guid;
            Description = payload.Description;
            Amount = payload.Amount.Value;
            Currency = payload.Amount.Currency;
            When = payload.When;
        }
    }
}
