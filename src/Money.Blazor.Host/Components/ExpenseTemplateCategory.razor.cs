using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components.Bootstrap;
using Money.Models;
using Money.Models.Queries;
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

namespace Money.Components;

public partial class ExpenseTemplateCategory(ICommandDispatcher Commands, IQueryDispatcher Queries)
{
    protected IKey EmptyCategoryKey { get; } = KeyFactory.Empty(typeof(Category));

    private IKey originalCategoryKey;

    protected List<string> ErrorMessages { get; } = new List<string>();
    protected List<CategoryModel> Categories { get; private set; }

    [Parameter]
    public IKey ExpenseTemplateKey { get; set; }

    [Parameter]
    public IKey CategoryKey { get; set; }

    protected async override Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        SetOriginal();
        Categories = await Queries.QueryAsync(new ListAllCategory());
    }

    private void SetOriginal()
    {
        originalCategoryKey = CategoryKey;
    }

    protected void OnSaveClick()
    {
        if (Validate())
        {
            Execute();
            SetOriginal();
            Modal.Hide();
        }
    }

    private bool Validate()
    {
        ErrorMessages.Clear();
        if (CategoryKey == null)
            ErrorMessages.Add("Category key must be set");

        return ErrorMessages.Count == 0;
    }

    private async void Execute()
    {
        await Commands.HandleAsync(new ChangeExpenseTemplateCategory(ExpenseTemplateKey, CategoryKey));
    }
}
