using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Models;
using Money.Services;
using Neptuo.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class ExpenseCardContext : ExpenseCard.IContext
    {
        [Inject]
        public ICommandDispatcher Commands { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }
        private CurrencyFormatter currencyFormatter;

        [Inject]
        protected Navigator Navigator { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsEditEnabled { get; set; } = true;

        #region OutcomeCard.IContext

        CurrencyFormatter ExpenseCard.IContext.CurrencyFormatter => currencyFormatter;

        bool ExpenseCard.IContext.HasEdit => IsEditEnabled;

        void ExpenseCard.IContext.Duplicate(IExpenseOverviewModel model)
            => OnActionClick<ModalDialog>(model, null, (modal, model) => Navigator.OpenExpenseCreate(model.Amount, model.Description, model.CategoryKey, model.When, model.IsFixed));

        void ExpenseCard.IContext.CreateTemplate(IExpenseOverviewModel model)
            => OnActionClick(model, TemplateCreateModal, (modal, model) => modal.Show(model.Amount, model.Description, model.CategoryKey, model.IsFixed));

        void ExpenseCard.IContext.EditAmount(IExpenseOverviewModel model)
            => OnActionClick(model, AmountEditModal);

        void ExpenseCard.IContext.EditDescription(IExpenseOverviewModel model)
            => OnActionClick(model, DescriptionEditModal);

        void ExpenseCard.IContext.EditWhen(IExpenseOverviewModel model)
            => OnActionClick(model, WhenEditModal);

        void ExpenseCard.IContext.Delete(IExpenseOverviewModel model)
            => OnDeleteClick(model);

        #endregion

        protected ExpenseTemplateCreate TemplateCreateModal { get; set; }
        protected ModalDialog AmountEditModal { get; set; }
        protected ModalDialog DescriptionEditModal { get; set; }
        protected ModalDialog WhenEditModal { get; set; }

        protected IExpenseOverviewModel SelectedItem { get; set; }
        protected string DeleteMessage { get; set; }
        protected Confirm DeleteConfirm { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            currencyFormatter = await CurrencyFormatterFactory.CreateAsync();
        }

        protected void OnActionClick<T>(IExpenseOverviewModel model, T modal, Action<T, IExpenseOverviewModel> showHandler = null)
            where T : ModalDialog
        {
            SelectedItem = model;
            if (showHandler == null)
                modal.Show();
            else
                showHandler(modal, model);

            StateHasChanged();
        }

        protected void OnDeleteClick(IExpenseOverviewModel model)
        {
            SelectedItem = model;
            DeleteMessage = $"Do you really want to delete expense '{model.Description}'?";
            DeleteConfirm.Show();
            StateHasChanged();
        }

        protected async void OnDeleteConfirmed()
        {
            await Commands.HandleAsync(new DeleteOutcome(SelectedItem.Key));
            StateHasChanged();
        }
    }
}
