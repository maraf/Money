using Money.Views.DesignData;
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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Money.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GroupPage : Page
    {
        public GroupPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = new ViewModelLocator().Group;
        }

        private void pvtGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //PivotItem item = (PivotItem)pvtGroups.ContainerFromIndex(pvtGroups.SelectedIndex);
            //if (item != null)
            //{
            //    ContentPresenter presenter = (ContentPresenter)item.FindName("copContent");
            //    if (presenter != null)
            //        presenter.Content = new OutcomePage();
            //}

            if (pvtGroups.SelectedIndex == 0)
                frmContent.Navigate(typeof(CategoryListPage));
            else 
                frmContent.Navigate(typeof(SummaryPage));

            //((Frame)Window.Current.Content).BackStack.Add(frmContent.BackStack.Last());
        }
    }
}
