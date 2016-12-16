using Money.Services;
using Money.Services.Models;
using Money.ViewModels;
using Money.Views.Navigation;
using Neptuo;
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
                    queryDispatcher = new DefaultQueryDispatcher();

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

        private GroupViewModel group;
        public GroupViewModel Group
        {
            get
            {
                if (group == null)
                {
                    group = new GroupViewModel(ServiceProvider.Navigator);
                    group.Items.Add(new GroupItemViewModel("August", null));
                    group.Items.Add(new GroupItemViewModel("September", null));
                    group.Items.Add(new GroupItemViewModel("October", null));
                    group.Items.Add(new GroupItemViewModel("December", null));
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
                    summary.Items.Add(new SummaryItemViewModel()
                    {
                        Name = "Food",
                        Color = Colors.Olive,
                        Amount = new Price(9540, "CZK"),
                    });
                    summary.Items.Add(new SummaryItemViewModel()
                    {
                        Name = "Eating Out",
                        Color = Colors.DarkRed,
                        Amount = new Price(3430, "CZK"),
                    });
                    summary.Items.Add(new SummaryItemViewModel()
                    {
                        Name = "Home",
                        Color = Colors.RosyBrown,
                        Amount = new Price(950, "CZK"),
                    });
                    summary.TotalAmount = new Price(13520, "CZK");
                    summary.IsLoading = false;
                }

                return summary;
            }
        }

        private CategoryOverviewViewModel categoryOverview;
        public CategoryOverviewViewModel CategoryOverview
        {
            get
            {
                if (categoryOverview == null)
                {
                    categoryOverview = new CategoryOverviewViewModel(ServiceProvider.Navigator, KeyFactory.Create(typeof(Category)), "Food", new MonthModel(2016, 11));
                    categoryOverview.Items.Add(new OutcomeOverviewModel(KeyFactory.Create(typeof(Outcome)), new Price(1250, "CZK"), new DateTime(2016, 11, 05), "Saturday's buy on market"));
                    categoryOverview.Items.Add(new OutcomeOverviewModel(KeyFactory.Create(typeof(Outcome)), new Price(350, "CZK"), new DateTime(2016, 11, 14), "Cheese"));
                    categoryOverview.Items.Add(new OutcomeOverviewModel(KeyFactory.Create(typeof(Outcome)), new Price(400, "CZK"), new DateTime(2016, 11, 15), "Vine"));
                    categoryOverview.Items.Add(new OutcomeOverviewModel(KeyFactory.Create(typeof(Outcome)), new Price(550, "CZK"), new DateTime(2016, 11, 15), "Pasta, pasta, pasta"));
                }

                return categoryOverview;
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
                    createOutcome.Categories.Add(new CategoryModel(KeyFactory.Create(typeof(Category)), "Food", null, Colors.CadetBlue));
                    createOutcome.Categories.Add(new CategoryModel(KeyFactory.Create(typeof(Category)), "Eating out", null, Colors.Brown));
                    createOutcome.Categories.Add(new CategoryModel(KeyFactory.Create(typeof(Category)), "Home", null, Colors.Gold));
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
                    categoryList = new CategoryListViewModel();
                    categoryList.Items.Add(new CategoryListItemViewModel(KeyFactory.Create(typeof(Category)), "Food", null, Colors.CadetBlue));
                    categoryList.Items.Add(new CategoryListItemViewModel(KeyFactory.Create(typeof(Category)), "Eating out", null, Colors.Brown) { IsSelected = true });
                    categoryList.Items.Add(new CategoryListItemViewModel(KeyFactory.Create(typeof(Category)), "Home", null, Colors.Gold));
                }

                return categoryList;
            }
        }

        public ViewModelLocator()
        {
            if (DesignMode.DesignModeEnabled)
            {
                ServiceProvider.QueryDispatcher = QueryDispatcher;
                ServiceProvider.DomainFacade = DomainFacade;
                ServiceProvider.Navigator = Navigator;
            }
        }
    }
}
