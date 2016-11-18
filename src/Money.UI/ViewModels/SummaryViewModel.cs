using Neptuo.Observables.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.ComponentModel;
using Neptuo.Activators;

namespace Money.ViewModels
{
    /// <summary>
    /// A view model for month or other over view.
    /// </summary>
    public class SummaryViewModel : ViewModel
    {
        private readonly IFactory<Price, decimal> priceFactory;

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
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

        public SummaryViewModel(IFactory<Price, decimal> priceFactory)
        {
            this.priceFactory = priceFactory;

            Items = new ObservableCollection<SummaryItemViewModel>();
            Items.CollectionChanged += OnItemsChanged;
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
