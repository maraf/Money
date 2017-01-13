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

        public Task AddOutcomeCategoryAsync(IKey outcomeKey, IKey categoryKey)
        {
            throw new NotImplementedException();
        }

        public Task ChangeCategoryColor(IKey categoryKey, Color color)
        {
            throw new NotImplementedException();
        }

        public Task ChangeCategoryDescription(IKey categoryKey, string description)
        {
            throw new NotImplementedException();
        }

        public Task ChangeOutcomeAmount(IKey outcomeKey, Price amount)
        {
            throw new NotImplementedException();
        }

        public Task ChangeOutcomeDescription(IKey outcomeKey, string description)
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

        public Task<TOutput> QueryAsync<TOutput>(IQuery<TOutput> query)
        {
            throw new NotImplementedException();
        }

        public Task RenameCategory(IKey categoryKey, string newName)
        {
            throw new NotImplementedException();
        }
    }
}
