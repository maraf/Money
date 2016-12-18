using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;

namespace Money.ViewModels
{
    public class CategoryListViewModel : ObservableObject
    {
        public ObservableCollection<CategoryEditViewModel> Items { get; private set; }
        public ICommand New { get; private set; }
        
        public CategoryListViewModel()
        {
            Items = new ObservableCollection<CategoryEditViewModel>();
            New = new DelegateCommand(NewExecuted);
        }

        private void NewExecuted()
        {

        }
    }
}
