using Money.ViewModels;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.Controls
{
    public class MainMenuEventArgs : EventArgs
    {
        public MenuItemViewModel Item { get; private set; }

        public MainMenuEventArgs(MenuItemViewModel item)
        {
            Ensure.NotNull(item, "item");
            Item = item;
        }
    }
}
