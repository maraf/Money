using Money.Services;
using Money.ViewModels.Commands;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using ICommand = System.Windows.Input.ICommand;

namespace Money.ViewModels
{
    public class CategoryEditViewModel : ObservableObject
    {
        public IKey Key { get; private set; }

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
            get { return new SolidColorBrush(ColorConverter.Map(Color)); }
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

        public ICommand Rename { get; private set; }
        public ICommand ChangeIcon { get; private set; }
        public ICommand ChangeColor { get; private set; }
        public ICommand Delete { get; private set; }

        public CategoryEditViewModel(ICommandDispatcher commandDispatcher, INavigator navigator, IKey key, string name, string description, Color color, string icon)
        {
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            Ensure.NotNull(navigator, "navigator");
            Ensure.Condition.NotEmptyKey(key);
            Key = key;
            Name = name;
            Description = description;
            Color = color;
            Icon = icon;

            Rename = new NavigateCommand(navigator, new CategoryRenameParameter(Key));
            ChangeIcon = new NavigateCommand(navigator, new CategoryChangeIconParameter(Key));
            ChangeColor = new NavigateCommand(navigator, new CategoryChangeColorParameter(Key));
            Delete = new CategoryDeleteCommand(navigator, commandDispatcher, Key);
        }
    }
}
