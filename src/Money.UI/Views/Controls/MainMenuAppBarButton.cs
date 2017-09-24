using Money.Views.StateTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Controls
{
    public class MainMenuAppBarButton : AppBarToggleButton
    {
        public MainMenuAppBarButton()
        {
            Label = "Menu";
            Icon = new FontIcon()
            {
                Glyph = "\uE700"
            };

            if (new MobileStateTrigger().IsActive)
                Visibility = Visibility.Visible;
            else
                Visibility = Visibility.Collapsed;
        }
    }
}
