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
    public class MainMenuAppBarButton : AppBarToggleButton, IDisposable
    {
        public MainMenuAppBarButton()
        {
            Label = "Menu";
            Icon = new FontIcon()
            {
                Glyph = "\uE700"
            };

            CoreWindow window = CoreWindow.GetForCurrentThread();
            window.SizeChanged += OnWindowSizeChanged;
            EnsureVisibility(window);
        }

        private void OnWindowSizeChanged(CoreWindow window, WindowSizeChangedEventArgs e)
        {
            EnsureVisibility(window);
        }

        private void EnsureVisibility(CoreWindow window)
        {
            if (new MobileStateTrigger().IsActive || window.Bounds.Width < (double)Application.Current.Resources["MediumSize"])
                Visibility = Visibility.Visible;
            else
                Visibility = Visibility.Collapsed;
        }

        public void Dispose()
        {
            CoreWindow window = CoreWindow.GetForCurrentThread();
            window.SizeChanged -= OnWindowSizeChanged;
        }
    }
}
