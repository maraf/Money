using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Models;
using Money.Services;
using Neptuo.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components;

public partial class IncomeCardContext(ICommandDispatcher Commands) : IncomeCard.IContext
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public bool IsEditEnabled { get; set; } = true;

    #region IncomeCard.IContext

    bool IncomeCard.IContext.HasEdit => IsEditEnabled;

    void IncomeCard.IContext.EditAmount(IncomeOverviewModel model)
        => OnActionClick(model, AmountEditModal);

    void IncomeCard.IContext.EditDescription(IncomeOverviewModel model)
        => OnActionClick(model, DescriptionEditModal);

    void IncomeCard.IContext.EditWhen(IncomeOverviewModel model)
        => OnActionClick(model, WhenEditModal);

    void IncomeCard.IContext.Delete(IncomeOverviewModel model)
        => OnDeleteClick(model);

    #endregion

    protected ModalDialog AmountEditModal { get; set; }
    protected ModalDialog DescriptionEditModal { get; set; }
    protected ModalDialog WhenEditModal { get; set; }

    protected IncomeOverviewModel SelectedItem { get; set; }
    protected string DeleteMessage { get; set; }
    protected Confirm DeleteConfirm { get; set; }

    protected void OnActionClick(IncomeOverviewModel model, ModalDialog modal)
    {
        SelectedItem = model;
        modal.Show();
        StateHasChanged();
    }

    protected void OnDeleteClick(IncomeOverviewModel model)
    {
        SelectedItem = model;
        DeleteMessage = $"Do you really want to delete income '{model.Description}'?";
        DeleteConfirm.Show();
        StateHasChanged();
    }

    protected async void OnDeleteConfirmed()
    {
        await Commands.HandleAsync(new DeleteIncome(SelectedItem.Key));
        StateHasChanged();
    }
}
