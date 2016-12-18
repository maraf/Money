using Money.Services;
using Neptuo;
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
        private readonly IDomainFacade domainFacade;

        public ObservableCollection<CategoryEditViewModel> Items { get; private set; }
        public ICommand New { get; private set; }
        
        public CategoryListViewModel(IDomainFacade domainFacade)
        {
            Ensure.NotNull(domainFacade, "domainFacade");
            this.domainFacade = domainFacade;

            Items = new ObservableCollection<CategoryEditViewModel>();
            New = new DelegateCommand(NewExecuted);
        }

        private void NewExecuted()
        {
            Items.Add(new CategoryEditViewModel(domainFacade, KeyFactory.Empty(typeof(Category))));
        }
    }
}
