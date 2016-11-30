using Neptuo.Activators;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    public partial class SummaryViewModel : ObservableObject
    {
        private readonly IFactory<Price, decimal> priceFactory;
        private readonly IProvider provider;
        private readonly ObservableCollection<SummaryItemViewModel> items;
        private bool isLoaded;
        
        public string Title { get; private set; }

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            internal set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Price totalAmount;
        public Price TotalAmount
        {
            get { return totalAmount; }
            set
            {
                if (totalAmount != value)
                {
                    totalAmount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public IEnumerable<SummaryItemViewModel> Items
        {
            get { return items; }
        }

        public SummaryViewModel(string title, IFactory<Price, decimal> priceFactory, IProvider provider)
        {
            this.priceFactory = priceFactory;
            this.provider = provider;
            this.items = new ObservableCollection<SummaryItemViewModel>();

            Title = title;
        }

        public async Task EnsureLoadedAsync()
        {
            if (isLoaded)
                return;

            IsLoading = true;

            items.Clear();

            await provider.ReplaceAsync(items);
            TotalAmount = await provider.GetTotalAmount();


            await Task.Delay(100);
            IsLoading = false;
            isLoaded = true;
        }
    }
}
