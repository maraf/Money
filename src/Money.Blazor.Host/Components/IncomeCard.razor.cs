using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Services;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components;

public partial class IncomeCard(Navigator Navigator)
{
    public interface IContext
    {
        bool HasEdit { get; }

        void EditAmount(IncomeOverviewModel model);
        void EditDescription(IncomeOverviewModel model);
        void EditWhen(IncomeOverviewModel model);
        void Delete(IncomeOverviewModel model);
    }

    [CascadingParameter]
    public IContext Context { get; set; }

    [Parameter]
    public IncomeOverviewModel Model { get; set; }

    [Parameter]
    public string CssClass { get; set; }

    protected void OnEditAmount()
        => Context.EditAmount(Model);

    protected void OnEditDescription()
        => Context.EditDescription(Model);

    protected void OnEditWhen()
        => Context.EditWhen(Model);

    protected void OnDelete()
        => Context.Delete(Model);
}
