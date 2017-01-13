using Money.Services;
using Money.Services.Models;
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

namespace Money.Bootstrap
{
    /// <summary>
    /// A default implementation of <see cref="IDomainFacade"/>.
    /// </summary>
    internal class DefaultDomainFacade : IDomainFacade
    {
        private readonly IRepository<Outcome, IKey> outcomeRepository;
        private readonly IRepository<Category, IKey> categoryRepository;

        public IFactory<Price, decimal> PriceFactory { get; private set; }

        public DefaultDomainFacade(
            IRepository<Outcome, IKey> outcomeRepository, 
            IRepository<Category, IKey> categoryRepository, 
            IFactory<Price, decimal> priceFactory)
        {
            Ensure.NotNull(outcomeRepository, "outcomeRepository");
            Ensure.NotNull(categoryRepository, "categoryRepository");
            Ensure.NotNull(priceFactory, "priceFactory");
            this.outcomeRepository = outcomeRepository;
            this.categoryRepository = categoryRepository;
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
        
        public Task RenameCategory(IKey categoryKey, string newName)
        {
            return Task.Factory.StartNew(() =>
            {
                Category category = categoryRepository.Get(categoryKey);
                category.Rename(newName);
                categoryRepository.Save(category);
            });
        }

        public Task ChangeCategoryDescription(IKey categoryKey, string description)
        {
            return Task.Factory.StartNew(() =>
            {
                Category category = categoryRepository.Get(categoryKey);
                category.ChangeDescription(description);
                categoryRepository.Save(category);
            });
        }

        public Task ChangeCategoryColor(IKey categoryKey, Color color)
        {
            return Task.Factory.StartNew(() =>
            {
                Category category = categoryRepository.Get(categoryKey);
                category.ChangeColor(color);
                categoryRepository.Save(category);
            });
        }

        public Task ChangeOutcomeAmount(IKey outcomeKey, Price amount)
        {
            return Task.Factory.StartNew(() =>
            {
                Outcome outcome = outcomeRepository.Get(outcomeKey);
                outcome.ChangeAmount(amount);
                outcomeRepository.Save(outcome);
            });
        }

        public Task ChangeOutcomeDescription(IKey outcomeKey, string description)
        {
            return Task.Factory.StartNew(() =>
            {
                Outcome outcome = outcomeRepository.Get(outcomeKey);
                outcome.ChangeDescription(description);
                outcomeRepository.Save(outcome);
            });
        }
    }
}
