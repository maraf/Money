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

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        #region OutcomeCard.IContext

        CurrencyFormatter ExpenseCard.IContext.CurrencyFormatter => currencyFormatter;

        bool ExpenseCard.IContext.HasEdit => true;

        void ExpenseCard.IContext.EditAmount(OutcomeOverviewModel model)
            => OnActionClick(model, AmountEditModal);

        void ExpenseCard.IContext.EditDescription(OutcomeOverviewModel model)
            => OnActionClick(model, DescriptionEditModal);

        void ExpenseCard.IContext.EditWhen(OutcomeOverviewModel model)
            => OnActionClick(model, WhenEditModal);

        void ExpenseCard.IContext.Delete(OutcomeOverviewModel model)
            => OnDeleteClick(model);

        #endregion

        protected ModalDialog AmountEditModal { get; set; }
        protected ModalDialog DescriptionEditModal { get; set; }
        protected ModalDialog WhenEditModal { get; set; }

        protected OutcomeOverviewModel SelectedItem { get; set; }
        protected string DeleteMessage { get; set; }
        protected Confirm DeleteConfirm { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            currencyFormatter = await CurrencyFormatterFactory.CreateAsync();
        }

        protected void OnActionClick(OutcomeOverviewModel model, ModalDialog modal)
        {
            SelectedItem = model;
            modal.Show();
            StateHasChanged();
        }

        protected void OnDeleteClick(OutcomeOverviewModel model)
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
