using Money.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Controls
{
    public class SummaryItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Category { get; set; }
        public DataTemplate Total { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            SummaryCategoryViewModel main = item as SummaryCategoryViewModel;
            if (main != null)
                return Category;

            SummaryTotalViewModel summary = item as SummaryTotalViewModel;
            if (summary != null)
                return Total;

            return base.SelectTemplateCore(item);
        }
    }
}
