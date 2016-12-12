using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Money.ViewModels
{
    public class CategoryListItemViewModel : ObservableObject
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

        public CategoryListItemViewModel(IKey key, string name, Color color)
        {
            Ensure.Condition.NotEmptyKey(key);
            Key = key;
            Name = name;
            Color = color;
        }
    }
}
