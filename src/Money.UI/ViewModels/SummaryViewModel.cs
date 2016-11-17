using Neptuo.Observables.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Money.ViewModels
{
    /// <summary>
    /// A view model for month or other over view.
    /// </summary>
    public class SummaryViewModel : ViewModel
    {
        private decimal totalAmount;
        public decimal TotalAmount
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

        public SummaryViewModel()
        {
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
            TotalAmount = Items.Sum(i => i.Amount);
        }
    }
}
