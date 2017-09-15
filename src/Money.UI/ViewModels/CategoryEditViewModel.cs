using Money.Services;
using Money.ViewModels.Commands;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Money.ViewModels
{
    public class CategoryEditViewModel : ObservableObject
    {
        private readonly IDomainFacade domainFacade;
        private readonly INavigator navigator;
        private IKey key;

        public IKey Key
        {
            get { return key; }
            set
            {
                key = value;
                EnsureCommands();
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Color color;
        public Color Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(ColorBrush));
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

        public Brush ColorBrush
        {
            get { return new SolidColorBrush(Color); }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ICommand changeIcon;
        public ICommand ChangeIcon
        {
            get { return changeIcon; }
            private set
            {
                if (changeIcon != value)
                {
                    changeIcon = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ICommand delete;
        public ICommand Delete
        {
            get { return delete; }
            private set
            {
                if (delete != value)
                {
                    delete = value;
                    RaisePropertyChanged();
                }
            }
        }

        public CategoryEditViewModel(IDomainFacade domainFacade, INavigator navigator, IKey key)
        {
            Ensure.NotNull(domainFacade, "domainFacade");
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(key, "key");
            this.domainFacade = domainFacade;
            this.navigator = navigator;

            Key = key;
            EnsureCommands();
        }

        public CategoryEditViewModel(IDomainFacade domainFacade, INavigator navigator, IKey key, string name, string description, Color color, string icon)
        {
            Ensure.NotNull(domainFacade, "domainFacade");
            Ensure.NotNull(navigator, "navigator");
            Ensure.Condition.NotEmptyKey(key);
            this.domainFacade = domainFacade;
            this.navigator = navigator;

            Key = key;
            Name = name;
            Description = description;
            Color = color;
            Icon = icon;
            EnsureCommands();
        }

        private void EnsureCommands()
        {
            if (!Key.IsEmpty)
            {
                if (ChangeIcon == null)
                    ChangeIcon = new NavigateCommand(navigator, new CategoryChangeIconParameter(Key));

                if (Delete == null)
                    Delete = new CategoryDeleteCommand(navigator, domainFacade, Key);
            }
        }
    }
}
