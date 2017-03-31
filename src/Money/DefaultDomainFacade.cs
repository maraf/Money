using Neptuo;
using Neptuo.Activators;
using Neptuo.Models.Keys;
using Neptuo.Models.Repositories;
using Neptuo.Queries;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Money
{
    /// <summary>
    /// A default implementation of <see cref="IDomainFacade"/>.
    /// </summary>
    public class DefaultDomainFacade : IDomainFacade
    {
        private readonly IRepository<Outcome, IKey> outcomeRepository;
        private readonly IRepository<Category, IKey> categoryRepository;
        private readonly IRepository<CurrencyList, IKey> currencyListRepository;

        private readonly IKey currencyListKey = GuidKey.Create(
            Guid.Parse("AF215C3D-B228-4004-806B-AC31398660A8"), 
            KeyFactory.Empty(typeof(CurrencyList)).Type
        );

        public IFactory<Price, decimal> PriceFactory { get; private set; }

        public DefaultDomainFacade(
            IRepository<Outcome, IKey> outcomeRepository, 
            IRepository<Category, IKey> categoryRepository,
            IRepository<CurrencyList, IKey> currencyListRepository,
            IFactory<Price, decimal> priceFactory)
        {
            Ensure.NotNull(outcomeRepository, "outcomeRepository");
            Ensure.NotNull(categoryRepository, "categoryRepository");
            Ensure.NotNull(currencyListRepository, "currencyListRepository");
            Ensure.NotNull(priceFactory, "priceFactory");
            this.outcomeRepository = outcomeRepository;
            this.categoryRepository = categoryRepository;
            this.currencyListRepository = currencyListRepository;
            PriceFactory = priceFactory;
        }

        public Task<IKey> CreateCategoryAsync(string name, Color color)
        {
            return Task.Factory.StartNew(() =>
            {
                Category model = new Category(name, color);
                categoryRepository.Save(model);
                return model.Key;
            });
        }

        public Task<IKey> CreateOutcomeAsync(Price amount, string description, DateTime when, IKey categoryKey)
        {
            return Task.Factory.StartNew(() =>
            {
                Outcome model = new Outcome(amount, description, when, categoryKey);
                outcomeRepository.Save(model);
                return model.Key;
            });
        }

        public Task AddOutcomeCategoryAsync(IKey outcomeKey, IKey categoryKey)
        {
            return Task.Factory.StartNew(() =>
            {
                Outcome model = outcomeRepository.Find(outcomeKey);
                model.AddCategory(categoryKey);
                outcomeRepository.Save(model);
            });
        }
        
        public Task RenameCategoryAsync(IKey categoryKey, string newName)
        {
            return Task.Factory.StartNew(() =>
            {
                Category category = categoryRepository.Get(categoryKey);
                category.Rename(newName);
                categoryRepository.Save(category);
            });
        }

        public Task ChangeCategoryDescriptionAsync(IKey categoryKey, string description)
        {
            return Task.Factory.StartNew(() =>
            {
                Category category = categoryRepository.Get(categoryKey);
                category.ChangeDescription(description);
                categoryRepository.Save(category);
            });
        }

        public Task ChangeCategoryColorAsync(IKey categoryKey, Color color)
        {
            return Task.Factory.StartNew(() =>
            {
                Category category = categoryRepository.Get(categoryKey);
                category.ChangeColor(color);
                categoryRepository.Save(category);
            });
        }

        public Task ChangeOutcomeAmountAsync(IKey outcomeKey, Price amount)
        {
            return Task.Factory.StartNew(() =>
            {
                Outcome outcome = outcomeRepository.Get(outcomeKey);
                outcome.ChangeAmount(amount);
                outcomeRepository.Save(outcome);
            });
        }

        public Task ChangeOutcomeDescriptionAsync(IKey outcomeKey, string description)
        {
            return Task.Factory.StartNew(() =>
            {
                Outcome outcome = outcomeRepository.Get(outcomeKey);
                outcome.ChangeDescription(description);
                outcomeRepository.Save(outcome);
            });
        }

        public Task ChangeOutcomeWhenAsync(IKey outcomeKey, DateTime when)
        {
            return Task.Factory.StartNew(() =>
            {
                Outcome outcome = outcomeRepository.Get(outcomeKey);
                outcome.ChangeWhen(when);
                outcomeRepository.Save(outcome);
            });
        }

        public Task DeleteOutcomeAsync(IKey outcomeKey)
        {
            return Task.Factory.StartNew(() =>
            {
                Outcome outcome = outcomeRepository.Get(outcomeKey);
                outcome.Delete();
                outcomeRepository.Save(outcome);
            });
        }

        public Task CreateCurrencyAsync(string name)
        {
            return Task.Factory.StartNew(() =>
            {
                CurrencyList currencies = currencyListRepository.Find(currencyListKey);
                if (currencies == null)
                    currencies = new CurrencyList();

                currencies.Add(name);
                currencyListRepository.Save(currencies);
            });
        }

        public Task SetCurrencyAsDefault(string name)
        {
            return Task.Factory.StartNew(() =>
            {
                CurrencyList currencies = currencyListRepository.Get(currencyListKey);
                currencies.SetAsDefault(name);
                currencyListRepository.Save(currencies);
            });
        }
    }
}
