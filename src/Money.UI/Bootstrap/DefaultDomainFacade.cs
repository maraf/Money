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
        public IQueryDispatcher QueryDispatcher { get; private set; }

        public DefaultDomainFacade(
            IRepository<Outcome, IKey> outcomeRepository, 
            IRepository<Category, IKey> categoryRepository, 
            IFactory<Price, decimal> priceFactory,
            IQueryDispatcher queryDispatcher)
        {
            Ensure.NotNull(outcomeRepository, "outcomeRepository");
            Ensure.NotNull(categoryRepository, "categoryRepository");
            Ensure.NotNull(priceFactory, "priceFactory");
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            this.outcomeRepository = outcomeRepository;
            this.categoryRepository = categoryRepository;
            PriceFactory = priceFactory;
            QueryDispatcher = queryDispatcher;
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
    }
}
