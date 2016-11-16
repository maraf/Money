using Money.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        Name = "Food"
                    });
                    summary.Items.Add(new SummaryItemViewModel()
                    {
                        Amount = 900,
                        Name = "Eating out"
                    });
                    summary.Items.Add(new SummaryItemViewModel()
                    {
                        Amount = 4400,
                        Name = "Home"
                    });
                }

                return summary;
            }
        }
    }
}
