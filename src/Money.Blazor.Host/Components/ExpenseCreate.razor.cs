using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Money.Pages;
using Money.Services;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class ExpenseCreate : System.IDisposable, IExpenseCreate
    {
        protected IKey EmptyCategoryKey { get; } = KeyFactory.Empty(typeof(Category));

        [Inject]
        protected Navigator.ComponentContainer ComponentContainer { get; set; }

        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }
        protected CurrencyFormatter CurrencyFormatter { get; set; }

        protected List<ExpenseTemplateModel> Templates { get; private set; }
        protected List<CategoryModel> Categories { get; private set; }
        protected List<CurrencyModel> Currencies { get; private set; }

        protected SelectedField Selected { get; set; } = SelectedField.Description;
        protected Price Amount { get; set; }
        protected string Description { get; set; }
        protected IKey CategoryKey { get; set; } = KeyFactory.Empty(typeof(Category));
        protected DateTime When { get; set; }

        private bool isAttachedToComponentContainer;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (ComponentContainer.ExpenseCreate == null)
            {
                ComponentContainer.ExpenseCreate = this;
                isAttachedToComponentContainer = true;
            }
        }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
        }

        public void Dispose()
        {
            if (isAttachedToComponentContainer)
                ComponentContainer.ExpenseCreate = null;
        }

        public void Show(IKey categoryKey)
        {
            base.Show();
            _ = LoadAsync();
        }

        public void Show(Price amount, string description, IKey categoryKey, bool isFixed)
        {
            base.Show();
            _ = LoadAsync();
        }

        protected async Task LoadAsync()
        {
            if (When == DateTime.MinValue)
                When = DateTime.Today;

            CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();
            Categories = await Queries.QueryAsync(new ListAllCategory());
            Currencies = await Queries.QueryAsync(new ListAllCurrency());
            StateHasChanged();
        }

        protected enum SelectedField
        {
            Amount,
            Description,
            Category,
            When
        }

        protected string FindCategoryName(IKey categoryKey)
        {
            var category = Categories.FirstOrDefault(c => c.Key.Equals(categoryKey));
            return category?.Name;
        }

        protected Color? FindCategoryColor(IKey categoryKey)
        {
            var category = Categories.FirstOrDefault(c => c.Key.Equals(categoryKey));
            return category?.Color;
        }
    }
}
