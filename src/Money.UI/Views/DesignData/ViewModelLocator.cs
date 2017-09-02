using Money.Services;
using Money.Services.Models;
using Money.Services.Tiles;
using Money.ViewModels;
using Money.ViewModels.Navigation;
using Neptuo;
using Neptuo.Events;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Money.Views.DesignData
{
    internal class ViewModelLocator
    {
        private IQueryDispatcher queryDispatcher;
        public IQueryDispatcher QueryDispatcher
        {
            get
            {
                if (queryDispatcher == null)
                    queryDispatcher = new QueryDispatcher();

                return queryDispatcher;
            }
        }

        private IDomainFacade domainFacade;
        public IDomainFacade DomainFacade
        {
            get
            {
                if (domainFacade == null)
                    domainFacade = new DomainFacade();

                return domainFacade;
            }
        }

        private INavigator navigator;
        public INavigator Navigator
        {
            get
            {
                if (navigator == null)
                    navigator = new Navigator();

                return navigator;
            }
        }

        private IEventHandlerCollection eventHandlers;
        public IEventHandlerCollection EventHandlers
        {
            get
            {
                if (eventHandlers == null)
                    eventHandlers = new DefaultEventManager();

                return eventHandlers;
            }
        }

        private GroupViewModel group;
        public GroupViewModel Group
        {
            get
            {
                if (group == null)
                {
                    group = new GroupViewModel(ServiceProvider.Navigator);
                    group.Add("August", null);
                    group.Add("September", null);
                    group.Add("October", null);
                    group.Add("December", null);
                }

                return group;
            }
        }

        private SummaryViewModel summary;
        public SummaryViewModel Summary
        {
            get
            {
                if (summary == null)
                {
                    summary = new SummaryViewModel(ServiceProvider.Navigator, ServiceProvider.QueryDispatcher);
                    summary.Items.Add(new SummaryCategoryViewModel()
                    {
                        Name = "Food",
                        Color = Colors.Olive,
                        Amount = new Price(9540, "CZK"),
                    });
                    summary.Items.Add(new SummaryCategoryViewModel()
                    {
                        Name = "Eating Out",
                        Color = Colors.DarkRed,
                        Amount = new Price(3430, "CZK"),
                    });
                    summary.Items.Add(new SummaryCategoryViewModel()
                    {
                        Name = "Home",
                        Color = Colors.RosyBrown,
                        Amount = new Price(950, "CZK"),
                    });
                    summary.Items.Add(new SummaryTotalViewModel(new Price(13520, "CZK")));
                    summary.IsLoading = false;
                }

                return summary;
            }
        }

        private OverviewViewModel overview;
        public OverviewViewModel Overview
        {
            get
            {
                if (overview == null)
                {
                    overview = new OverviewViewModel(ServiceProvider.Navigator, KeyFactory.Create(typeof(Category)), "Food", new MonthModel(2016, 11));
                    overview.Items.Add(new OutcomeOverviewViewModel(new OutcomeOverviewModel(KeyFactory.Create(typeof(Outcome)), new Price(1250, "CZK"), new DateTime(2016, 11, 05), "Saturday's buy on market")));
                    overview.Items.Add(new OutcomeOverviewViewModel(new OutcomeOverviewModel(KeyFactory.Create(typeof(Outcome)), new Price(350, "CZK"), new DateTime(2016, 11, 14), "Cheese")));
                    overview.Items.Add(new OutcomeOverviewViewModel(new OutcomeOverviewModel(KeyFactory.Create(typeof(Outcome)), new Price(400, "CZK"), new DateTime(2016, 11, 15), "Vine")));
                    overview.Items.Add(new OutcomeOverviewViewModel(new OutcomeOverviewModel(KeyFactory.Create(typeof(Outcome)), new Price(550, "CZK"), new DateTime(2016, 11, 15), "Pasta, pasta, pasta")));
                    overview.Items[2].IsSelected = true;
                }

                return overview;
            }
        }

        private OutcomeViewModel createOutcome;
        public OutcomeViewModel CreateOutcome
        {
            get
            {
                if (createOutcome == null)
                {
                    createOutcome = new OutcomeViewModel(ServiceProvider.Navigator, ServiceProvider.DomainFacade);
                    createOutcome.Amount = 5400;
                    createOutcome.Description = "New home PC motherboard";
                    createOutcome.Categories.Add(new CategoryModel(KeyFactory.Create(typeof(Category)), "Food", "Making out loved foods from igredients", Colors.CadetBlue));
                    createOutcome.Categories.Add(new CategoryModel(KeyFactory.Create(typeof(Category)), "Eating out", "When we are lay and let others to feed us", Colors.Brown));
                    createOutcome.Categories.Add(new CategoryModel(KeyFactory.Create(typeof(Category)), "Home", "Manly stuff", Colors.Gold));
                }

                return createOutcome;
            }
        }

        private CategoryListViewModel categoryList;
        public CategoryListViewModel CategoryList
        {
            get
            {
                if (categoryList == null)
                {
                    categoryList = new CategoryListViewModel(ServiceProvider.DomainFacade);
                    categoryList.Items.Add(new CategoryEditViewModel(ServiceProvider.DomainFacade, KeyFactory.Create(typeof(Category)), "Food", "Making out loved foods from igredients", Colors.CadetBlue));
                    categoryList.Items.Add(new CategoryEditViewModel(ServiceProvider.DomainFacade, KeyFactory.Create(typeof(Category)), "Eating out", "When we are lazy and let others to feed us", Colors.Brown) { IsSelected = true });
                    categoryList.Items.Add(new CategoryEditViewModel(ServiceProvider.DomainFacade, KeyFactory.Create(typeof(Category)), "Home", "Manly stuff", Colors.Gold));
                }

                return categoryList;
            }
        }

        private CategoryEditViewModel categoryEdit;
        public CategoryEditViewModel CategoryEdit
        {
            get
            {
                if (categoryEdit == null)
                {
                    categoryEdit = new CategoryEditViewModel(ServiceProvider.DomainFacade, KeyFactory.Create(typeof(Category)), "Eating out", "When we are lazy and let others to feed us", Colors.Brown);
                    categoryEdit.IsSelected = true;
                }

                return categoryEdit;
            }
        }

        private CurrencyListViewModel currencyList;
        public CurrencyListViewModel CurrencyList
        {
            get
            {
                if (currencyList == null)
                {
                    currencyList = new CurrencyListViewModel(ServiceProvider.DomainFacade, ServiceProvider.Navigator);
                    currencyList.Items.Add(new CurrencyEditViewModel(ServiceProvider.Navigator, ServiceProvider.DomainFacade, ServiceProvider.QueryDispatcher, "CZK", "Kč"));
                    currencyList.Items.Add(new CurrencyEditViewModel(ServiceProvider.Navigator, ServiceProvider.DomainFacade, ServiceProvider.QueryDispatcher, "USD", "$"));
                    currencyList.Items.Add(new CurrencyEditViewModel(ServiceProvider.Navigator, ServiceProvider.DomainFacade, ServiceProvider.QueryDispatcher, "EUR", "€"));
                    currencyList.Items.First().IsSelected = true;
                    currencyList.Items.First().ExchangeRates.Add(new ExchangeRateModel("USD", 18.90, new DateTime(2016, 10, 11)));
                    currencyList.Items.First().ExchangeRates.Add(new ExchangeRateModel("EUR", 27.40, new DateTime(2017, 4, 21)));
                }

                return currencyList;
            }
        }

        private CurrencyEditViewModel currencyEdit;
        public CurrencyEditViewModel CurrencyEdit
        {
            get
            {
                if (currencyEdit == null)
                {
                    currencyEdit = new CurrencyEditViewModel(ServiceProvider.Navigator, ServiceProvider.DomainFacade, ServiceProvider.QueryDispatcher, "CZK", "Kč");
                    currencyEdit.IsSelected = true;
                    currencyEdit.ExchangeRates.Add(new ExchangeRateModel("USD", 18.90, new DateTime(2016, 10, 11)));
                    currencyEdit.ExchangeRates.Add(new ExchangeRateModel("EUR", 27.40, new DateTime(2017, 4, 21)));
                }

                return currencyEdit;
            }
        }

        private MigrateViewModel migrate;
        public MigrateViewModel Migrate
        {
            get
            {
                if (migrate == null)
                {
                    migrate = new MigrateViewModel(new UpgradeService());
                    migrate.StartAsync();
                }

                return migrate;
            }
        }

        public ViewModelLocator()
        {
            if (DesignMode.DesignModeEnabled)
            {
                ServiceProvider.QueryDispatcher = QueryDispatcher;
                ServiceProvider.DomainFacade = DomainFacade;
                ServiceProvider.EventHandlers = EventHandlers;
                ServiceProvider.Navigator = Navigator;
                ServiceProvider.TileService = new TileService();
            }
        }
    }
}
