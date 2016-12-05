using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Money.Views.Controls
{
    [ContentProperty(Name = "PrimaryCommands")]
    public sealed partial class MainCommandBar : CommandBar
    {
        public MainCommandBar()
        {
            this.InitializeComponent();
        }

        private void OnPieChartClicked(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(GroupTemplate), 1, new Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo());
            //((Frame)Window.Current.Content).BackStack.Add(new PageStackEntry(typeof(GroupPage), 1, new DrillInNavigationTransitionInfo()));
        }
    }
}
