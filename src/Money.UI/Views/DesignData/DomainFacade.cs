using Money.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Activators;

namespace Money.Views.DesignData
{
    internal class DomainFacade : IDomainFacade
    {
        public IFactory<Price, decimal> PriceFactory
        {
            get { throw new NotImplementedException(); }
        }

        public Task CreateOutcomeAsync(Price amount, string description, DateTime when)
        {
            throw new NotImplementedException();
        }
    }
}
