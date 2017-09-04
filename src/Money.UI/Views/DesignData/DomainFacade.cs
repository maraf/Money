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

        public Task ChangeCategoryColorAsync(IKey categoryKey, Color color)
        {
            throw new NotImplementedException();
        }

        public Task ChangeCategoryDescriptionAsync(IKey categoryKey, string description)
        {
            throw new NotImplementedException();
        }

        public Task ChangeCategoryIconAsync(IKey categoryKey, string icon)
        {
            throw new NotImplementedException();
        }

        public Task ChangeCurrencySymbolAsync(string uniqueCode, string symbol)
        {
            throw new NotImplementedException();
        }

        public Task ChangeOutcomeAmountAsync(IKey outcomeKey, Price amount)
        {
            throw new NotImplementedException();
        }

        public Task ChangeOutcomeDescriptionAsync(IKey outcomeKey, string description)
        {
            throw new NotImplementedException();
        }

        public Task ChangeOutcomeWhenAsync(IKey outcomeKey, DateTime when)
        {
            throw new NotImplementedException();
        }

        public Task<IKey> CreateCategoryAsync(string name, Color color)
        {
            throw new NotImplementedException();
        }

        public Task CreateCurrencyAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task CreateCurrencyAsync(string uniqueCode, string symbol)
        {
            throw new NotImplementedException();
        }

        public Task<IKey> CreateOutcomeAsync(Price amount, string description, DateTime when, IKey categoryKey)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCurrencyAsync(string uniqueCode)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOutcomeAsync(IKey outcomeKey)
        {
            throw new NotImplementedException();
        }

        public Task<TOutput> QueryAsync<TOutput>(IQuery<TOutput> query)
        {
            throw new NotImplementedException();
        }

        public Task RenameCategoryAsync(IKey categoryKey, string newName)
        {
            throw new NotImplementedException();
        }

        public Task SetCurrencyAsDefaultAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task SetExchangeRateAsync(string sourceName, string targetName, DateTime validFrom, double rate)
        {
            throw new NotImplementedException();
        }
    }
}
