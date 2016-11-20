using Money.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Activators;
using Money.Services.Models;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using Windows.UI;

namespace Money.Views.DesignData
{
    internal class DomainFacade : IDomainFacade
    {
        public IFactory<Price, decimal> PriceFactory
        {
            get { throw new NotImplementedException(); }
        }

        public IQueryDispatcher QueryDispatcher
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Task CreateCategoryAsync(string name, Color color)
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
