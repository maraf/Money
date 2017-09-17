using Money.Events;
using Money.ViewModels.Commands;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Neptuo;
using Neptuo.Events.Handlers;
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

namespace Money.ViewModels
{
    public class CategoryListViewModel : ObservableObject, 
        IEventHandler<CategoryCreated>,
        IEventHandler<CategoryRenamed>,
        IEventHandler<CategoryDescriptionChanged>,
        IEventHandler<CategoryIconChanged>,
        IEventHandler<CategoryDeleted>
    {
        private readonly IDomainFacade domainFacade;
        private readonly INavigator navigator;

        public ObservableCollection<CategoryEditViewModel> Items { get; private set; }
        public ICommand New { get; private set; }
        
        public CategoryListViewModel(IDomainFacade domainFacade, INavigator navigator)
        {
            Ensure.NotNull(domainFacade, "domainFacade");
            Ensure.NotNull(navigator, "navigator");
            this.domainFacade = domainFacade;
            this.navigator = navigator;

            Items = new ObservableCollection<CategoryEditViewModel>();
            New = new NavigateCommand(navigator, new CategoryCreateParameter());
        }

        public Task HandleAsync(CategoryCreated payload)
        {
            CategoryEditViewModel viewModel = new CategoryEditViewModel(domainFacade, navigator, payload.AggregateKey, payload.Name, null, payload.Color, null);
            Items.Add(viewModel);
            return Task.CompletedTask;
        }

        public Task HandleAsync(CategoryRenamed payload)
        {
            CategoryEditViewModel viewModel = Items.FirstOrDefault(vm => vm.Key.Equals(payload.AggregateKey));
            if (viewModel != null)
                viewModel.Name = payload.NewName;

            return Task.CompletedTask;
        }

        public Task HandleAsync(CategoryDescriptionChanged payload)
        {
            CategoryEditViewModel viewModel = Items.FirstOrDefault(vm => vm.Key.Equals(payload.AggregateKey));
            if (viewModel != null)
                viewModel.Description = payload.Description;

            return Task.CompletedTask;
        }

        public Task HandleAsync(CategoryIconChanged payload)
        {
            CategoryEditViewModel viewModel = Items.FirstOrDefault(vm => vm.Key.Equals(payload.AggregateKey));
            if (viewModel != null)
                viewModel.Icon = payload.Icon;

            return Task.CompletedTask;
        }

        public Task HandleAsync(CategoryDeleted payload)
        {
            CategoryEditViewModel viewModel = Items.FirstOrDefault(vm => vm.Key.Equals(payload.AggregateKey));
            if (viewModel != null)
                Items.Remove(viewModel);

            return Task.CompletedTask;
        }
    }
}
