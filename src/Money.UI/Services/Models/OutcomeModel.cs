using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services.Models
{
    /// <summary>
    /// A model of single outcome.
    /// </summary>
    public class OutcomeModel
    {
        public IKey Key { get; private set; }
        public Price Amount { get; private set; }
        public string Description { get; private set; }
        public IEnumerable<IKey> CategoryKeys { get; private set; }
    }
}
