using Money.Services.Models;
using Money.ViewModels;
using Neptuo;
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
        private SummaryViewModel summary;
        public SummaryViewModel Summary
        {
            get
            {
                if (summary == null)
                {
                    summary = new SummaryViewModel();
                    summary.Groups.Add(new SummaryGroupViewModel("August", new PriceFactory("CZK"), new SummaryGroupViewModelProvider()));
                    summary.Groups.Add(new SummaryGroupViewModel("September", new PriceFactory("CZK"), new SummaryGroupViewModelProvider()));
                    summary.Groups.Last().EnsureLoadedAsync().Wait();
                    summary.Groups.Add(new SummaryGroupViewModel("October", new PriceFactory("CZK"), new SummaryGroupViewModelProvider()));
                    summary.Groups.Add(new SummaryGroupViewModel("November", new PriceFactory("CZK"), new SummaryGroupViewModelProvider()));
                }

                return summary;
            }
        }

        private ListViewModel list;
        public ListViewModel List
        {
            get
            {
                if (list == null)
                {
                    list = new ListViewModel();
                    list.GroupId = Guid.NewGuid();
                    list.Name = "Eating";
                    list.Items.Add(new ListItemViewModel()
                    {
                        Description = "Saturday's buy on market",
                        Amount = new Price(1250, "CZK"),
                        When = new DateTime(2016, 11, 12, 10, 30, 15)
                    });
                    list.Items.Add(new ListItemViewModel()
                    {
                        Description = "Cheese",
                        Amount = new Price(345, "CZK"),
                        When = new DateTime(2016, 11, 13, 20, 0, 0)
                    });
                }

                return list;
            }
        }

        private OutcomeViewModel createOutcome;
        public OutcomeViewModel CreateOutcome
        {
            get
            {
                if (createOutcome == null)
                {
                    createOutcome = new OutcomeViewModel(new DomainFacade());
                    createOutcome.Amount = 5400;
                    createOutcome.Description = "New home PC motherboard";
                    createOutcome.Categories.Add(new CategoryModel(KeyFactory.Create(typeof(Category)), "Food", Colors.CadetBlue));
                    createOutcome.Categories.Add(new CategoryModel(KeyFactory.Create(typeof(Category)), "Eating out", Colors.Brown));
                    createOutcome.Categories.Add(new CategoryModel(KeyFactory.Create(typeof(Category)), "Home", Colors.Gold));
                }

                return createOutcome;
            }
        }
    }
}
