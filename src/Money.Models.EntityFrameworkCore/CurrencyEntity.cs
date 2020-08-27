using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class CurrencyEntity : IUserEntity
    {
        public string UserId { get; set; }
        public string UniqueCode { get; set; }
        public string Symbol { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
    }
}
