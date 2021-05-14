 using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// A model of a single outcome.
    /// </summary>
    public class OutcomeModel : IPriceFixed
    {
        public IKey Key { get; private set; }
        public Price Amount { get; private set; }
        public DateTime When { get; set; }
        public string Description { get; private set; }
        public IEnumerable<IKey> CategoryKeys { get; private set; }
        public bool IsFixed { get; set; }

        public OutcomeModel(IKey key, Price amount, DateTime when, string description, IEnumerable<IKey> categoryKeys, bool isFixed)
        {
            Key = key;
            Amount = amount;
            Description = description;
            When = when;
            CategoryKeys = categoryKeys;
            IsFixed = isFixed;
        }
    }
}
