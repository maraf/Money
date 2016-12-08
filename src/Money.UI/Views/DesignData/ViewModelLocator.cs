using Money.Services;
using Money.Services.Models;
using Money.ViewModels;
using Money.Views.Navigation;
using Neptuo;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    group = new GroupViewModel(Navigator);
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
                    summary = new SummaryViewModel(QueryDispatcher);
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

        private CategoryListViewModel categoryList;
        public CategoryListViewModel CategoryList
        {
            get
            {
                if (categoryList == null)
                {
                    categoryList = new CategoryListViewModel(Navigator);
                    categoryList.GroupId = Guid.NewGuid();
                    categoryList.Name = "Eating";
                    categoryList.Items.Add(new CategoryListItemViewModel()
                    {
                        Description = "Saturday's buy on market",
                        Amount = new Price(1250, "CZK"),
                        When = new DateTime(2016, 11, 12, 10, 30, 15)
                    });
                    categoryList.Items.Add(new CategoryListItemViewModel()
                    {
                        Description = "Cheese",
                        Amount = new Price(345, "CZK"),
                        When = new DateTime(2016, 11, 13, 20, 0, 0)
                    });
                }

                return categoryList;
            }
        }

        private OutcomeViewModel createOutcome;
        public OutcomeViewModel CreateOutcome
        {
            get
            {
                if (createOutcome == null)
                {
                    createOutcome = new OutcomeViewModel(Navigator, DomainFacade);
                    createOutcome.Amount = 5400;
                    createOutcome.Description = "New home PC motherboard";
                    createOutcome.Categories.Add(new CategoryModel(KeyFactory.Create(typeof(Category)), "Food", Colors.CadetBlue));
                    createOutcome.Categories.Add(new CategoryModel(KeyFactory.Create(typeof(Category)), "Eating out", Colors.Brown));
                    createOutcome.Categories.Add(new CategoryModel(KeyFactory.Create(typeof(Category)), "Home", Colors.Gold));
                }

                return createOutcome;
            }
        }

        public ViewModelLocator()
        {
            ServiceProvider.QueryDispatcher = QueryDispatcher;
            ServiceProvider.DomainFacade = DomainFacade;
            ServiceProvider.Navigator = Navigator;
        }
    }
}
