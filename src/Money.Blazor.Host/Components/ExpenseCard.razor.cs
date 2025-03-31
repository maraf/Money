using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components;

public partial class ExpenseCard(Navigator Navigator)
{
    public interface IContext
    {
        bool HasEdit { get; }

        void CreateTemplate(IExpenseOverviewModel model);
        void EditAmount(IExpenseOverviewModel model);
        void EditDescription(IExpenseOverviewModel model);
        void EditWhen(IExpenseOverviewModel model);
        void Delete(IExpenseOverviewModel model);
    }

    [Parameter]
    [CascadingParameter]
    public IContext Context { get; set; }

    [Parameter]
    public IExpenseOverviewModel Model { get; set; }

    [Parameter]
    public string AmountCssClass { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        AmountCssClass = string.Join(" ", [AmountCssClass, "m-0"]);
    }

    protected void OnDuplicate()
        => Navigator.OpenExpenseCreate(Model.Amount, Model.Description, Model.CategoryKey, Model.When, Model.IsFixed);

    protected void OnCreateTemplate()
        => Context.CreateTemplate(Model);

    protected void OnEditAmount() 
        => Context.EditAmount(Model);

    protected void OnEditDescription() 
        => Context.EditDescription(Model);

    protected void OnEditWhen() 
        => Context.EditWhen(Model);

    protected void OnDelete() 
        => Context.Delete(Model);
}
