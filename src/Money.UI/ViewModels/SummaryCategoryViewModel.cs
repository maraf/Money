using Money.Services.Models.Queries;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Observables;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Money.ViewModels
{
    /// <summary>
    /// A view model for an item of summary.
    /// </summary>
    public class SummaryCategoryViewModel : ObservableObject, ISummaryItemViewModel
    {
        private readonly IQueryDispatcher queryDispatcher;

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
                    AmountValue = (double)value.Value;
                    queryDispatcher.QueryAsync(new GetCurrencySymbol(amount.Currency)).ContinueWith(t => CurrencySymbol = t.Result);
                }
            }
        }

        private string currencySymbol;
        public string CurrencySymbol
        {
            get { return currencySymbol; }
            set
            {
                if (currencySymbol != value)
                {
                    currencySymbol = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double amountValue;
        public double AmountValue
        {
            get { return amountValue; }
            set
            {
                if (amountValue != value)
                {
                    amountValue = value;
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

        public SummaryCategoryViewModel(IQueryDispatcher queryDispatcher)
        {
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            this.queryDispatcher = queryDispatcher;
        }
    }
}
