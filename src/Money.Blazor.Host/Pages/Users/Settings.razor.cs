using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components.Bootstrap;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
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
        protected Modal PriceDecimalsEditor { get; set; }

        protected PropertyViewModel DateFormat { get; set; }
        protected Modal DateFormatEditor { get; set; }

        protected PropertyViewModel MobileMenu { get; set; }
        protected Modal MobileMenuEditor { get; set; }
        protected List<IAvailableMenuItemModel> MobileMenuAvailableModels { get; set; }
        protected List<string> MobileSelectedIdentifiers { get; set; }

        protected List<UserPropertyModel> Models { get; set; }
        protected List<PropertyViewModel> ViewModels { get; } = new List<PropertyViewModel>();

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            EventHandlers.Add<UserPropertyChanged>(this);

            PriceDecimals = AddProperty("PriceDecimalDigits", "Price decimal digits", () => PriceDecimalsEditor.Show(), icon: "pound-sign", defaultValue: "2");
            DateFormat = AddProperty("DateFormat", "Date format", () => DateFormatEditor.Show(), icon: "calendar-day", defaultValue: CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern);
            MobileMenu = AddProperty("MobileMenu", "Mobile menu", () => MobileMenuEditor.Show(), icon: "mobile");

            MobileMenuAvailableModels = await Queries.QueryAsync(new ListAvailableMenuItem());

            await LoadAsync();

            MobileSelectedIdentifiers = MobileMenu.CurrentValue != null 
                ? MobileMenu.CurrentValue.Split(',').ToList()
                : new List<string>(0);
        }

        public void Dispose()
        {
            EventHandlers.Remove<UserPropertyChanged>(this);
        }

        private PropertyViewModel AddProperty(string name, string title, Action edit, string defaultValue = null, string icon = null)
        {
            var viewModel = ViewModels.FirstOrDefault(vm => vm.Key == name);
            if (viewModel == null)
                ViewModels.Add(viewModel = new PropertyViewModel(Commands));

            viewModel.Key = name;
            viewModel.Title = title;
            viewModel.Icon = icon;
            viewModel.Edit = edit;
            viewModel.DefaultValue = defaultValue;

            return viewModel;
        }

        protected async Task SetMobileMenuAsync()
        {
            MobileMenu.CurrentValue = String.Join(",", MobileMenuAvailableModels.Where(m => MobileSelectedIdentifiers.Contains(m.Identifier)).Select(m => m.Identifier)); 
            await MobileMenu.SetAsync(); 
            MobileMenuEditor.Hide();
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
        }
    }

    public class PropertyViewModel
    {
        private readonly ICommandDispatcher commands;

        public PropertyViewModel(ICommandDispatcher commands)
        {
            Ensure.NotNull(commands, "commands");
            this.commands = commands;
        }

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

        public async Task SetAsync()
        {
            Console.WriteLine($"Current '{currentValue}', ModelValue '{Model?.Value}'.");

            if (String.IsNullOrEmpty(currentValue) || currentValue == DefaultValue)
                currentValue = null;

            if (Model == null || currentValue != Model.Value)
            {
                Console.WriteLine("Send command.");
                await commands.HandleAsync(new SetUserProperty(Key, currentValue));
            }
        }
    }
}
