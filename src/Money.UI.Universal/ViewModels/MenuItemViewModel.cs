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

        private string group;
        public string Group
        {
            get { return group; }
            set
            {
                if (group != value)
                {
                    group = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public object Parameter { get; private set; }

        [Obsolete("Designer only.")]
        public MenuItemViewModel()
        { }

        public MenuItemViewModel(string label, object parameter)
        {
            Ensure.NotNull(parameter, "parameter");
            Label = label;
            Parameter = parameter;
            Parameter = parameter;
        }

        public MenuItemViewModel(string label, string icon, object parameter)
            : this(label, parameter)
        {
            Icon = icon;
        }
    }
}
