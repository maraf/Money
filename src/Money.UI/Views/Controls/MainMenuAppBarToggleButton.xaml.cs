using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Money.Views.Controls
{
    public sealed partial class MainMenuAppBarToggleButton : AppBarToggleButton, IDisposable
    {
        public MainMenuAppBarToggleButton()
        {
            InitializeComponent();

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
            Visibility = window.Bounds.Width < (double)Application.Current.Resources["MediumSize"] 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }

        public void Dispose()
        {
            CoreWindow window = CoreWindow.GetForCurrentThread();
            window.SizeChanged -= OnWindowSizeChanged;
        }
    }
}
