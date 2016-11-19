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

namespace Money.Bootstrap
{
    /// <summary>
    /// A default implementation of <see cref="IDomainFacade"/>.
    /// </summary>
    internal class DefaultDomainFacade : IDomainFacade
    {
        private readonly IRepository<Outcome, IKey> outcomeRepository;

        public IFactory<Price, decimal> PriceFactory { get; private set; }

        public DefaultDomainFacade(IRepository<Outcome, IKey> outcomeRepository, IFactory<Price, decimal> priceFactory)
        {
            Ensure.NotNull(outcomeRepository, "outcomeRepository");
            Ensure.NotNull(priceFactory, "priceFactory");
            this.outcomeRepository = outcomeRepository;
            PriceFactory = priceFactory;
        }

        public Task CreateOutcomeAsync(Price amount, string description, DateTime when)
        {
            return Task.Factory.StartNew(() =>
            {
                Outcome model = new Outcome(amount, description, when);
                outcomeRepository.Save(model);
            });
        }
    }
}
