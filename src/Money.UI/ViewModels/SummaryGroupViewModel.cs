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
    public partial class SummaryGroupViewModel : ObservableObject
    {
        private readonly IFactory<Price, decimal> priceFactory;
        private readonly IProvider provider;
        private bool isLoaded;
        
        public string Title { get; private set; }

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            private set
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

        public ObservableCollection<SummaryItemViewModel> Items { get; private set; }

        public SummaryGroupViewModel(string title, IFactory<Price, decimal> priceFactory, IProvider provider)
        {
            this.priceFactory = priceFactory;
            this.provider = provider;

            Title = title;
            Items = new ObservableCollection<SummaryItemViewModel>();
            Items.CollectionChanged += OnItemsChanged;
        }

        public async Task EnsureLoadedAsync()
        {
            if (isLoaded)
                return;

            IsLoading = true;
            Items.Clear();
            await provider.ReplaceAsync(Items);
            isLoaded = true;
            IsLoading = false;
        }

        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SummaryItemViewModel item in e.NewItems)
                    item.PropertyChanged += OnItemPropertyChanged;
            }

            if (e.OldItems != null)
            {
                foreach (SummaryItemViewModel item in e.OldItems)
                    item.PropertyChanged -= OnItemPropertyChanged;
            }

            UpdateTotalAmount();
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SummaryItemViewModel.Amount))
                UpdateTotalAmount();
        }

        private void UpdateTotalAmount()
        {
            TotalAmount = priceFactory.Create(0);
            foreach (SummaryItemViewModel item in Items)
                TotalAmount += item.Amount;
        }
    }
}
