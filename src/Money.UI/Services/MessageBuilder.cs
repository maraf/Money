using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class MessageBuilder
    {
        public string CurrencyAlreadyExists(string uniqueCode) => String.Format("A currency with code '{0}' already exists.", uniqueCode);
    }
}
