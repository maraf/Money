using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Money.Views
{
    [NavigationParameter(typeof(AboutParameter))]
    public sealed partial class About : Page
    {
        public About()
        {
            InitializeComponent();
            ReloadException();
        }

        private void ReloadException()
        {
            if (ApplicationData.Current.LocalSettings.Containers.TryGetValue("Exception", out ApplicationDataContainer container))
            {
                ExceptionPanel.Visibility = Visibility.Visible;

                if (container.Values.TryGetValue("Type", out object type))
                    ExceptionType.Text = (string)type;

                if (container.Values.TryGetValue("Callstack", out object message))
                    ExceptionMessage.Text = (string)message;

                if (container.Values.TryGetValue("Callstack", out object callstack))
                    ExceptionCallstack.Text = (string)callstack;

                if (container.Values.TryGetValue("DateTime", out object dateTime))
                    ExceptionCallstack.Text = (string)dateTime;
            }
            else
            {
                ExceptionPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void ClearException_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.DeleteContainer("Exception");
            ReloadException();
        }
    }
}
