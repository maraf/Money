using Money.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

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
                    summary.Items.Add(new SummaryItemViewModel()
                    {
                        Amount = 2500,
                        Name = "Food",
                        Color = Colors.CadetBlue
                    });
                    summary.Items.Add(new SummaryItemViewModel()
                    {
                        Amount = 900,
                        Name = "Eating out",
                        Color = Colors.Brown
                    });
                    summary.Items.Add(new SummaryItemViewModel()
                    {
                        Amount = 4400,
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
                        Amount = 75.43M,
                        When = new DateTime(2016, 11, 12, 10, 30, 15)
                    });
                    list.Items.Add(new ListItemViewModel()
                    {
                        Description = "Cheese",
                        Amount = 12.55M,
                        When = new DateTime(2016, 11, 13, 20, 0, 0)
                    });
                }

                return list;
            }
        }
    }
}
