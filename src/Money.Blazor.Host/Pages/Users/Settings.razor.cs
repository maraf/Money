using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components;
using Money.Components.Bootstrap;
using Money.Components.Settings;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.Models.Sorting;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages.Users
{
    public partial class Settings : System.IDisposable,
        IEventHandler<UserPropertyChanged>
    {
        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected IEventHandlerCollection EventHandlers { get; set; }

        protected PropertyViewModel PriceDecimals { get; set; }
        protected PropertyDialog PriceDecimalsEditor { get; set; }

        protected PropertyViewModel DateFormat { get; set; }
        protected PropertyDialog DateFormatEditor { get; set; }

        protected MobileMenuPropertyViewModel MobileMenu { get; set; }
        protected PropertyDialog MobileMenuEditor { get; set; }

        protected SortPropertyViewModel<SummarySortType> SummarySort { get; set; }
        protected PropertyDialog SummarySortEditor { get; set; }

        protected List<UserPropertyModel> Models { get; set; }
        protected List<PropertyViewModel> ViewModels { get; } = new List<PropertyViewModel>();

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            EventHandlers.Add<UserPropertyChanged>(this);

            PriceDecimals = AddProperty("PriceDecimalDigits", "Price decimal digits", () => PriceDecimalsEditor.Show(), icon: "pound-sign", defaultValue: "2");
            DateFormat = AddProperty("DateFormat", "Date format", () => DateFormatEditor.Show(), icon: "calendar-day", defaultValue: CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern);
            MobileMenu = AddProperty<MobileMenuPropertyViewModel>("MobileMenu", "Mobile menu", () => MobileMenuEditor.Show(), icon: "mobile");
            SummarySort = AddProperty<SortPropertyViewModel<SummarySortType>>("SummarySort", "Summary sort", () => SummarySortEditor.Show(), icon: "sort-alpha-down", defaultValue: "ByCategory-Ascending");

            await LoadAsync();
        }

        public void Dispose()
        {
            EventHandlers.Remove<UserPropertyChanged>(this);
        }

        private PropertyViewModel AddProperty(string name, string title, Action edit, string defaultValue = null, string icon = null)
            => AddProperty<PropertyViewModel>(name, title, edit, defaultValue, icon);

        private T AddProperty<T>(string name, string title, Action edit, string defaultValue = null, string icon = null)
            where T : PropertyViewModel, new()
        {
            var viewModel = ViewModels.FirstOrDefault(vm => vm.Key == name);
            if (viewModel == null)
            {
                ViewModels.Add(viewModel = new T()
                {
                    Commands = Commands,
                    Queries = Queries
                });
            }

            viewModel.Key = name;
            viewModel.Title = title;
            viewModel.Icon = icon;
            viewModel.Edit = edit;
            viewModel.DefaultValue = defaultValue;

            return (T)viewModel;
        }

        Task IEventHandler<UserPropertyChanged>.HandleAsync(UserPropertyChanged payload)
        {
            var viewModel = ViewModels.FirstOrDefault(vm => vm.Key == payload.PropertyKey);
            if (viewModel != null)
            {
                viewModel.Model.Value = payload.Value;
                viewModel.CurrentValue = null;
            }

            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task LoadAsync()
        {
            Models = await Queries.QueryAsync(new ListUserProperty());
            foreach (var model in Models)
            {
                var viewModel = ViewModels.FirstOrDefault(vm => vm.Key == model.Key);
                if (viewModel != null)
                    viewModel.Model = model;
            }

            foreach (var viewModel in ViewModels)
                await viewModel.InitializeAsync();
        }
    }

    public class PropertyViewModel
    {
        public ICommandDispatcher Commands { get; set; }
        public IQueryDispatcher Queries { get; set; }

        public string Key { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public Action Edit { get; set; }
        public UserPropertyModel Model { get; set; }
        public string DefaultValue { get; set; }

        private string currentValue = null;

        public string CurrentValue
        {
            get => currentValue ?? Model?.Value ?? DefaultValue;
            set => currentValue = value;
        }

        public virtual async Task SetAsync()
        {
            Console.WriteLine($"Current '{currentValue}', ModelValue '{Model?.Value}'.");

            if (String.IsNullOrEmpty(currentValue) || currentValue == DefaultValue)
                currentValue = null;

            if (Model == null || currentValue != Model.Value)
            {
                Console.WriteLine("Send command.");
                await Commands.HandleAsync(new SetUserProperty(Key, currentValue));
            }
        }

        public virtual Task InitializeAsync()
            => Task.CompletedTask;
    }

    public class MobileMenuPropertyViewModel : PropertyViewModel
    {
        public List<IAvailableMenuItemModel> AvailableModels { get; set; }
        public List<string> SelectedIdentifiers { get; set; }

        public async override Task InitializeAsync()
        {
            await base.InitializeAsync();

            AvailableModels = await Queries.QueryAsync(new ListAvailableMenuItem());

            SelectedIdentifiers = CurrentValue != null
                ? CurrentValue.Split(',').ToList()
                : new List<string>(0);
        }

        public override Task SetAsync()
        {
            CurrentValue = String.Join(",", AvailableModels.Where(m => SelectedIdentifiers.Contains(m.Identifier)).Select(m => m.Identifier));
            return base.SetAsync();
        }
    }

    public class SortPropertyViewModel<T> : PropertyViewModel
        where T : struct
    {
        public List<(string Name, T Value)> Properties { get; set; }
        public T Property { get; set; }
        public List<(string Name, SortDirection Value)> Directions { get; set; }
        public SortDirection Direction { get; set; }

        public async override Task InitializeAsync()
        {
            await base.InitializeAsync();

            Properties = new List<(string Name, T Value)>();
            SortButton<T>.BuildItems(Properties);

            Directions = new List<(string Name, SortDirection Value)>();
            SortButton<SortDirection>.BuildItems(Directions);

            if (CurrentValue != null)
            {
                string[] parts = CurrentValue.Split('-');
                Property = Enum.Parse<T>(parts[0]);
                Direction = Enum.Parse<SortDirection>(parts[1]);
            }
        }

        public override Task SetAsync()
        {
            CurrentValue = $"{Property}-{Direction}";
            return base.SetAsync();
        }
    }
}
