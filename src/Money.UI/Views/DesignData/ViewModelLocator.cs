using Money.ViewModels;
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
                    summary.Title = "September 2016";
                    summary.Items.Add(new SummaryItemViewModel()
                    {
                        Amount = new Price(2500, "CZK"),
                        Name = "Food",
                        Color = Colors.CadetBlue
                    });
                    summary.Items.Add(new SummaryItemViewModel()
                    {
                        Amount = new Price(900, "CZK"),
                        Name = "Eating out",
                        Color = Colors.Brown
                    });
                    summary.Items.Add(new SummaryItemViewModel()
                    {
                        Amount = new Price(4400, "CZK"),
                        Name = "Home",
                        Color = Colors.Gold
                    });
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
                    createOutcome = new OutcomeViewModel();
                    createOutcome.Amount = 5400;
                    createOutcome.Description = "New home PC motherboard";
                    createOutcome.Categories.Add(new CategoryViewModel()
                    {
                        Name = "Food",
                        Color = Colors.CadetBlue
                    });
                    createOutcome.Categories.Add(new CategoryViewModel()
                    {
                        Name = "Eating out",
                        Color = Colors.Brown
                    });
                    createOutcome.Categories.Add(new CategoryViewModel()
                    {
                        Name = "Home",
                        Color = Colors.Gold
                    });
                }

                return createOutcome;
            }
        }
    }
}
