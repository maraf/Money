using Money.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Activators;
using Money.Services.Models;
using Neptuo.Models.Keys;

namespace Money.Views.DesignData
{
    internal class DomainFacade : IDomainFacade
    {
        public IFactory<Price, decimal> PriceFactory
        {
            get { throw new NotImplementedException(); }
        }

        public Task CreateCategoryAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task CreateOutcomeAsync(Price amount, string description, DateTime when)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OutcomeModel> ListOutcomeByCategory(IKey categoryKey)
        {
            throw new NotImplementedException();
        }
    }
}
