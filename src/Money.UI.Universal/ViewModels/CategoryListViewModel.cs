using Money.Events;
using Money.ViewModels.Commands;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Events.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ICommand = System.Windows.Input.ICommand;

namespace Money.ViewModels
{
    public class CategoryListViewModel : ObservableObject, 
        IEventHandler<CategoryCreated>,
        IEventHandler<CategoryRenamed>,
        IEventHandler<CategoryDescriptionChanged>,
        IEventHandler<CategoryColorChanged>,
        IEventHandler<CategoryIconChanged>,
        IEventHandler<CategoryDeleted>
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly INavigator navigator;

        public ObservableCollection<CategoryEditViewModel> Items { get; private set; }
        public ICommand New { get; private set; }
        
        public CategoryListViewModel(ICommandDispatcher commandDispatcher, INavigator navigator)
        {
            Ensure.NotNull(commandDispatcher, "commandDispatcher");
            Ensure.NotNull(navigator, "navigator");
            this.commandDispatcher = commandDispatcher;
            this.navigator = navigator;

            Items = new ObservableCollection<CategoryEditViewModel>();
            New = new NavigateCommand(navigator, new CategoryCreateParameter());
        }

        private Task UpdateItem(IKey categoryKey, Action<CategoryEditViewModel> handler)
        {
            CategoryEditViewModel viewModel = Items.FirstOrDefault(vm => vm.Key.Equals(categoryKey));
            if (viewModel != null)
                handler(viewModel);

            return Task.CompletedTask;
        }

        public Task HandleAsync(CategoryCreated payload)
        {
            CategoryEditViewModel viewModel = new CategoryEditViewModel(commandDispatcher, navigator, payload.AggregateKey, payload.Name, null, payload.Color, null);
            Items.Add(viewModel);
            return Task.CompletedTask;
        }

        public Task HandleAsync(CategoryRenamed payload)
        {
            return UpdateItem(payload.AggregateKey, viewModel => viewModel.Name = payload.NewName);
        }

        public Task HandleAsync(CategoryDescriptionChanged payload)
        {
            return UpdateItem(payload.AggregateKey, viewModel => viewModel.Description = payload.Description);
        }

        public Task HandleAsync(CategoryColorChanged payload)
        {
            return UpdateItem(payload.AggregateKey, viewModel => viewModel.Color = payload.Color);
        }

        public Task HandleAsync(CategoryIconChanged payload)
        {
            return UpdateItem(payload.AggregateKey, viewModel => viewModel.Icon = payload.Icon);
        }

        public Task HandleAsync(CategoryDeleted payload)
        {
            return UpdateItem(payload.AggregateKey, viewModel => Items.Remove(viewModel));
        }
    }
}
