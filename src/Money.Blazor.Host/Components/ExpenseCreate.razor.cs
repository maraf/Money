using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic;
using Money.Commands;
using Money.Models;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Pages;
using Money.Services;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Logging;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class ExpenseCreate : System.IDisposable, IExpenseCreateNavigator
    {
        protected IKey EmptyCategoryKey { get; } = KeyFactory.Empty(typeof(Category));

        [Inject]
        protected Interop Interop { get; set; }

        [Inject]
        protected Navigator Navigator { get; set; }

        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        internal ILog<ExpenseCreate> Log { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }
        protected CurrencyFormatter CurrencyFormatter { get; set; }

        [Parameter][CascadingParameter]
        public Navigator.ComponentContainer ComponentContainer { get; set; }

        protected Confirm PrerequisitesConfirm { get; set; }

        protected List<ExpenseTemplateModel> Templates { get; private set; }
        protected List<CategoryModel> Categories { get; private set; }
        protected List<CurrencyModel> Currencies { get; private set; }

        protected ErrorMessages Errors { get; } = ErrorMessages.Create();

        protected SelectedField Selected { get; set; } = SelectedField.Description;
        protected Price Amount { get; set; }
        protected string Description { get; set; }
        protected List<ExpenseTemplateModel> SuggestedTemplates { get; set; } = new();
        protected IKey CategoryKey { get; set; } = KeyFactory.Empty(typeof(Category));
        protected DateTime When { get; set; }
        protected bool IsFixed { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (ComponentContainer != null)
            {
                Log.Debug("Attach to component container");
                ComponentContainer.ExpenseCreate = this;
            }
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (FocusAfterRender)
            {
                string elementId = Selected switch 
                {
                    SelectedField.Description => "expense-wiz-description",
                    SelectedField.Amount => "expense-wiz-amount",
                    SelectedField.Category => !CategoryKey.IsEmpty ? $"expense-wiz-category-{CategoryKey.AsGuidKey().Guid.ToString()}" : null,
                    SelectedField.When => "expense-wiz-when",
                    _ => null,
                };

                if (!String.IsNullOrEmpty(elementId))
                    await Interop.FocusElementByIdAsync(elementId);

                FocusAfterRender = false;
            }
        }

        protected bool FocusAfterRender;

        public void Dispose()
        {
            Log.Debug("Dispose");
            if (ComponentContainer != null && ComponentContainer.ExpenseCreate == this)
                ComponentContainer.ExpenseCreate = null;
        }

        private async void ShowInternal(Func<bool> setParameters = null)
        {
            FocusAfterRender = true;
            
            bool suggestTemplates = setParameters?.Invoke() ?? true;

            await LoadAsync();

            if (suggestTemplates)
                SuggestTemplates();

            StateHasChanged();
            
            if (Currencies == null || Currencies.Count == 0 || Categories == null || Categories.Count == 0)
                PrerequisitesConfirm.Show();
            else
                base.Show();
        }

        public new void Show() 
            => ShowInternal();

        public void Show(IKey categoryKey) => ShowInternal(() =>
        {
            CategoryKey = categoryKey;
            return true;
        });

        public void Show(Price amount, string description, IKey categoryKey, bool isFixed) => ShowInternal(() =>
        {
            Amount = amount;
            Description = description;
            CategoryKey = categoryKey;
            IsFixed = isFixed;
            return false;
        });

        public void Show(Price amount, string description, IKey categoryKey, DateTime when, bool isFixed) => ShowInternal(() =>
        {
            Amount = amount;
            Description = description;
            CategoryKey = categoryKey;
            When = when;
            IsFixed = isFixed;
            return false;
        });

        protected async Task LoadAsync()
        {
            if (When == DateTime.MinValue)
                When = DateTime.Today;

            CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();
            Categories = await Queries.QueryAsync(new ListAllCategory());
            Currencies = await Queries.QueryAsync(new ListAllCurrency());
            Templates = await Queries.QueryAsync(ListAllExpenseTemplate.Version3());
            StateHasChanged();
        }

        protected IEnumerable<ExpenseTemplateModel> GetTemplates()
        {
            if (Templates == null)
                return Enumerable.Empty<ExpenseTemplateModel>();

            return /*SuggestedTemplates.Count == 0 || */string.IsNullOrEmpty(Description) ? Templates : SuggestedTemplates;
        }

        protected enum SelectedField
        {
            Amount,
            Description,
            Category,
            When
        }

        protected record ErrorMessages(List<string> Amount, List<string> Description, List<string> Category, List<string> When) : IEnumerable<string>
        {
            public void Clear()
            {
                Amount.Clear();
                Description.Clear();
                Category.Clear();
                When.Clear();
            }

            public IEnumerable<string> Get(SelectedField field) => field switch 
            {
                SelectedField.Amount => Amount,
                SelectedField.Description => Description,
                SelectedField.Category => Category,
                SelectedField.When => When,
                _ => throw new InvalidOperationException($"The '{field}' is not supported")
            };

            public IEnumerable<string> All() => Enumerable.Concat(Amount, Enumerable.Concat(Description, Enumerable.Concat(Category, When)));

            public bool IsEmpty() => Amount.Count == 0 && Description.Count == 0 && Category.Count == 0 && When.Count == 0;

            public IEnumerator<string> GetEnumerator() => All().GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => All().GetEnumerator();

            public static ErrorMessages Create() => new(new(1), new(1), new(1), new(1));
        }

        protected void SuggestTemplates()
        {
            SuggestedTemplates.Clear();
            string description = Description?.ToLowerInvariant();
            if (String.IsNullOrEmpty(description))
                return;

            foreach (var template in Templates)
            {
                string templateDescription = template.Description?.ToLowerInvariant();
                if (String.IsNullOrEmpty(templateDescription))
                    continue;

                if (templateDescription.Contains(description))
                    SuggestedTemplates.Add(template);
            }

            if (suggestExistingExpensesCancellation != null)
                suggestExistingExpensesCancellation.Cancel();

            suggestExistingExpensesCancellation = new();
            _ = SuggestExistingExpensesAsync(description, suggestExistingExpensesCancellation.Token);
        }

        private CancellationTokenSource suggestExistingExpensesCancellation;
        private SortDescriptor<OutcomeOverviewSortType> suggestExistingExpensesSort = new(OutcomeOverviewSortType.ByWhen, SortDirection.Descending);

        protected async Task SuggestExistingExpensesAsync(string description, CancellationToken ct)
        {
            var results = await Queries.QueryAsync(new SearchOutcomes(description, suggestExistingExpensesSort, 0));
            if (ct.IsCancellationRequested)
                return;

            foreach (var result in results)
            {
                if (SuggestedTemplates.Any(t => IsSimilar(t, result)))
                    continue;

                SuggestedTemplates.Add(new ExpenseTemplateModel(
                    KeyFactory.Create(typeof(ExpenseTemplate)),
                    result.Amount,
                    result.Description,
                    result.CategoryKey
                ));
            }

            StateHasChanged();

            static bool IsSimilar(ExpenseTemplateModel template, OutcomeOverviewModel other) 
                => template.Amount == other.Amount
                    && template.Description == other.Description
                    && template.CategoryKey.Equals(other.CategoryKey);
        }

        protected void ApplyTemplate(ExpenseTemplateModel model)
        {
            Amount = model.Amount;
            Description = model.Description;
            CategoryKey = model.CategoryKey;
            IsFixed = model.IsFixed;

            SuggestedTemplates.Clear();
            Validate(ErrorMessages.Create());
        }

        protected void SetSelectedField(SelectedField selected, bool focusAfterRender = true)
        {
            Selected = selected;
            FocusAfterRender = focusAfterRender;
        }

        protected async Task CreateAsync()
        {
            if (!Validate(Errors))
                return;

            await ExecuteAsync();

            Hide();

            await Task.Delay(200);

            SuggestedTemplates.Clear();
            Description = null;
            Amount = null;
            CategoryKey = EmptyCategoryKey;
            When = DateTime.UtcNow.Date;
            IsFixed = false;
            SetSelectedField(SelectedField.Description, false);
        }

        private Task ExecuteAsync()
        {
            return Commands.HandleAsync(new CreateOutcome(Amount, Description, When, CategoryKey, IsFixed));
        }

        private bool Validate(ErrorMessages errors)
        {
            Log.Debug($"Expense: Amount: {Amount}, Category: {CategoryKey}, When: {When}.");

            void ValidateField(SelectedField field, Func<bool> validator)
            {
                bool areMessagesEmpty = errors.IsEmpty();
                bool isInvalid = validator();
                if (areMessagesEmpty && isInvalid)
                    SetSelectedField(field);
            }

            errors.Clear();
            ValidateField(SelectedField.Description, () => Validator.AddOutcomeDescription(errors.Description, Description));
            ValidateField(SelectedField.Amount, () => Validator.AddOutcomeAmount(errors.Amount, Amount?.Value ?? 0));
            ValidateField(SelectedField.Category, () => Validator.AddOutcomeCategoryKey(errors.Category, CategoryKey));

            Log.Debug($"Expense: Validation: '{string.Join("', '", errors)}'.");
            return errors.IsEmpty();
        }

        protected void OnPrerequisitesConfirmed()
        {
            if (Currencies == null || Currencies.Count == 0)
                Navigator.OpenCurrencies();
            else if (Categories == null || Categories.Count == 0)
                Navigator.OpenCategories();
        }
    }
}
