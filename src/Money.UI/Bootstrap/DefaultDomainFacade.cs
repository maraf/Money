using Money.Services;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Models.Keys;
using Neptuo.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Money.Services.Models;
using Neptuo.Queries;
using Windows.UI;
using Neptuo.Threading.Tasks;

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

        public Task CreateCategoryAsync(string name, Color color)
        {
            return Task.Factory.StartNew(() =>
            {
                Category model = new Category(name, color);
                categoryRepository.Save(model);
            });
        }

        public Task CreateOutcomeAsync(Price amount, string description, DateTime when)
        {
            return Task.Factory.StartNew(() =>
            {
                Outcome model = new Outcome(amount, description, when);
                outcomeRepository.Save(model);
            });
        }

        public IEnumerable<OutcomeModel> ListOutcomeByCategory(IKey categoryKey)
        {
            throw new NotImplementedException();
        }
    }
}
