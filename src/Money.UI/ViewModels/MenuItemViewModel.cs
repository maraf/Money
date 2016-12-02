using Neptuo;
using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Money.ViewModels
{
    public class MenuItemViewModel : ObservableObject
    {
        private string label;
        public string Label
        {
            get { return label; }
            set
            {
                if (label != value)
                {
                    label = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string icon;
        public string Icon
        {
            get { return icon; }
            set
            {
                if (icon != value)
                {
                    icon = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Type Page { get; private set; }
        public object Parameter { get; private set; }

        public MenuItemViewModel(string label, Type page, object parameter = null)
        {
            Ensure.NotNull(page, "page");
            Label = label;
            Page = page;
            Parameter = parameter;
        }

        public MenuItemViewModel(string label, string icon, Type page, object parameter = null)
        {
            Ensure.NotNull(page, "page");
            Label = label;
            Icon = icon;
            Page = page;
            Parameter = parameter;
        }
    }
}
