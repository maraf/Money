using Money.ViewModels.Parameters;
using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    public class MainMenuListFactory : IFactory<IReadOnlyList<MenuItemViewModel>, bool>
    {
        public IReadOnlyList<MenuItemViewModel> Create(bool isDesktop)
        {
            List<MenuItemViewModel> result = new List<MenuItemViewModel>();

            result.Add(new MenuItemViewModel("Month", "\uE908", new SummaryParameter()) { Group = "Summary" });
            if (!isDesktop)
                result.Add(new MenuItemViewModel("Create Outcome", "\uE108", new OutcomeParameter()) { Group = "Summary" });

            result.Add(new MenuItemViewModel("Categories", "\uE8FD", new CategoryListParameter()) { Group = "Manage" });
            result.Add(new MenuItemViewModel("Currencies", "\uE1D0", new CurrencyParameter()) { Group = "Manage" });

            result.Add(new MenuItemViewModel("Settings", "\uE713", new AboutParameter()) { Group = "Settings" });


            return result;
        }
    }
}
