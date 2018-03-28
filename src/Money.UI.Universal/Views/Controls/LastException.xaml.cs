using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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

namespace Money.Views.Controls
{
    public sealed partial class LastException : StackPanel
    {
        private readonly IFactory<ApplicationDataContainer> storageContainerFactory = ServiceProvider.StorageContainerFactory;

        public LastException()
        {
            InitializeComponent();
            ReloadException();
        }

        private void ReloadException()
        {
            var exception = FindException();
            if (exception != null)
            {
                Visibility = Visibility.Visible;

                if (!String.IsNullOrEmpty(exception.Value.type))
                    ExceptionType.Text = exception.Value.type;

                if (!String.IsNullOrEmpty(exception.Value.message))
                    ExceptionMessage.Text = exception.Value.message;

                if (!String.IsNullOrEmpty(exception.Value.callstack))
                    ExceptionCallstack.Text = exception.Value.callstack;

                if (!String.IsNullOrEmpty(exception.Value.dateTime))
                    ExceptionDateTime.Text = exception.Value.dateTime;

                StringBuilder url = new StringBuilder();
                url.Append("https://github.com/maraf/Money/issues/new");
                url.Append("?title=Unhandled exception");
                url.Append("&body=");
                url.Append($"**Type:**{Environment.NewLine}{exception.Value.type ?? "empty"}");
                url.Append(Environment.NewLine);
                url.Append(Environment.NewLine);
                url.Append($"**Message:**{Environment.NewLine}{exception.Value.message ?? "empty"}");
                url.Append(Environment.NewLine);
                url.Append(Environment.NewLine);
                url.Append($"**CallStack:**{Environment.NewLine}{exception.Value.callstack ?? "empty"}");
                url.Append(Environment.NewLine);
                url.Append(Environment.NewLine);
                url.Append($"**DateTime:**{Environment.NewLine}{exception.Value.dateTime ?? "empty"}");

                btnReport.NavigateUri = new Uri(url.ToString().Replace(Environment.NewLine, "%0D%0A"), UriKind.Absolute);
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private (string type, string message, string callstack, string dateTime)? FindException()
        {
            if (storageContainerFactory.Create().Containers.TryGetValue("Exception", out ApplicationDataContainer container))
            {
                if (container.Values.TryGetValue("Type", out object type))
                    ExceptionType.Text = (string)type;

                if (container.Values.TryGetValue("Message", out object message))
                    ExceptionMessage.Text = (string)message;

                if (container.Values.TryGetValue("Callstack", out object callstack))
                    ExceptionCallstack.Text = (string)callstack;

                if (container.Values.TryGetValue("DateTime", out object dateTime))
                    ExceptionCallstack.Text = (string)dateTime;

                return ((string)type, (string)message, (string)callstack, (string)dateTime);
            }

            return null;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            storageContainerFactory.Create().DeleteContainer("Exception");
            ReloadException();
        }
    }
}
