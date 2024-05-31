using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Models;
using Money.Models.Queries;
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
        protected Navigator.ComponentContainer ComponentContainer { get; set; }

        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Inject]
        internal ILog<ExpenseCreate> Log { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }
        protected CurrencyFormatter CurrencyFormatter { get; set; }

        protected List<ExpenseTemplateModel> Templates { get; private set; }
        protected List<CategoryModel> Categories { get; private set; }
        protected List<CurrencyModel> Currencies { get; private set; }

        protected ErrorMessages Errors { get; } = new(new(1), new(1), new(1), new(1), new(1));

        protected SelectedField Selected { get; set; } = SelectedField.Description;
        protected Price Amount { get; set; }
        protected string Description { get; set; }
        protected List<ExpenseTemplateModel> SuggestedTemplates { get; set; } = new();
        protected IKey CategoryKey { get; set; } = KeyFactory.Empty(typeof(Category));
        protected DateTime When { get; set; }
        protected bool IsFixed { get; set; }

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
                    SelectedField.Flags => "expense-wiz-fixed",
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
            if (isAttachedToComponentContainer)
                ComponentContainer.ExpenseCreate = null;
        }

        private void ShowInternal(Action setParameters = null)
        {
            base.Show();
            FocusAfterRender = true;
            
            setParameters?.Invoke();

            _ = LoadAsync();
        }

        public new void Show() 
            => ShowInternal();

        public void Show(IKey categoryKey) 
            => ShowInternal(() => CategoryKey = categoryKey);

        public void Show(Price amount, string description, IKey categoryKey, bool isFixed) => ShowInternal(() =>
        {
            Amount = amount;
            Description = description;
            CategoryKey = categoryKey;
            IsFixed = isFixed;
        });

        protected async Task LoadAsync()
        {
            if (When == DateTime.MinValue)
                When = DateTime.Today;

            CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();
            Categories = await Queries.QueryAsync(new ListAllCategory());
            Currencies = await Queries.QueryAsync(new ListAllCurrency());
            Templates = await Queries.QueryAsync(new ListAllExpenseTemplate());
            StateHasChanged();
        }

        protected IEnumerable<ExpenseTemplateModel> GetTemplates()
        {
            if (Templates == null)
                return Enumerable.Empty<ExpenseTemplateModel>();

            return SuggestedTemplates.Count == 0 ? Templates : SuggestedTemplates;
        }

        protected enum SelectedField
        {
            Amount,
            Description,
            Category,
            When,
            Flags
        }

        protected record ErrorMessages(List<string> Amount, List<string> Description, List<string> Category, List<string> When, List<string> Flags) : IEnumerable<string>
        {
            public void Clear()
            {
                Amount.Clear();
                Description.Clear();
                Category.Clear();
                When.Clear();
                Flags.Clear();
            }

            public IEnumerable<string> Get(SelectedField field) => field switch 
            {
                SelectedField.Amount => Amount,
                SelectedField.Description => Description,
                SelectedField.Category => Category,
                SelectedField.When => When,
                SelectedField.Flags => Flags,
                _ => throw new InvalidOperationException($"The '{field}' is not supported")
            };

            public IEnumerable<string> All() => Enumerable.Concat(Amount, Enumerable.Concat(Description, Enumerable.Concat(Category, Enumerable.Concat(When, Flags))));

            public bool IsEmpty() => Amount.Count == 0 && Description.Count == 0 && Category.Count == 0 && When.Count == 0 && Flags.Count == 0;

            public IEnumerator<string> GetEnumerator() => All().GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => All().GetEnumerator();
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
                if (String.IsNullOrEmpty(description))
                    continue;

                if (templateDescription.Contains(description))
                    SuggestedTemplates.Add(template);
            }
        }

        protected void ApplyTemplate(ExpenseTemplateModel model)
        {
            Amount = model.Amount;
            Description = model.Description;
            CategoryKey = model.CategoryKey;
            IsFixed = model.IsFixed;

            SuggestedTemplates.Clear();
            SetSelectedField(SelectedField.Description);
        }

        protected void SetSelectedField(SelectedField selected, bool focusAfterRender = true)
        {
            Selected = selected;
            FocusAfterRender = focusAfterRender;
        }

        protected async Task CreateAsync()
        {
            if (!Validate())
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

        private bool Validate()
        {
            Log.Debug($"Expense: Amount: {Amount}, Category: {CategoryKey}, When: {When}.");

            void ValidateField(SelectedField field, Func<bool> validator)
            {
                bool areMessagesEmpty = Errors.IsEmpty();
                bool isInvalid = validator();
                if (areMessagesEmpty && isInvalid)
                    SetSelectedField(field);
            }

            Errors.Clear();
            ValidateField(SelectedField.Description, () => Validator.AddOutcomeDescription(Errors.Description, Description));
            ValidateField(SelectedField.Amount, () => Validator.AddOutcomeAmount(Errors.Amount, Amount?.Value ?? 0));
            ValidateField(SelectedField.Category, () => Validator.AddOutcomeCategoryKey(Errors.Category, CategoryKey));

            Log.Debug($"Expense: Validation: '{string.Join("', '", Errors)}'.");
            return Errors.IsEmpty();
        }
    }
}
