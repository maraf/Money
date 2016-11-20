using Money.UI;
using Neptuo.Models.Keys;
using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Money.ViewModels
{
    /// <summary>
    /// A view model for an item of summary.
    /// </summary>
    public class SummaryItemViewModel : ObservableObject
    {
        private IKey categoryKey;
        public IKey CategoryKey
        {
            get { return categoryKey; }
            set
            {
                if (categoryKey != value)
                {
                    categoryKey = value;
                    RaisePropertyChanged();
                }
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

        private Price amount;
        public Price Amount
        {
            get { return amount; }
            set
            {
                if (amount != value)
                {
                    amount = value;
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
                }
            }
        }
    }
}
