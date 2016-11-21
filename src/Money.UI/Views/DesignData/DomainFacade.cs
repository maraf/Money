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

        public Task AddOutcomeCategoryAsync(IKey outcomeKey, IKey categoryKey)
        {
            throw new NotImplementedException();
        }

        public Task<IKey> CreateCategoryAsync(string name, Color color)
        {
            throw new NotImplementedException();
        }

        public Task<IKey> CreateOutcomeAsync(Price amount, string description, DateTime when, IKey categoryKey)
        {
            throw new NotImplementedException();
        }
    }
}
