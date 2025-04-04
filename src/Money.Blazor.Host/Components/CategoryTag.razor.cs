using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components;

public partial class CategoryTag(IQueryDispatcher Queries)
{
    [Parameter]
    public IKey Key { get; set; }

    [Parameter]
    public string CssClass { get; set; }

    protected string Name { get; set; }
    protected Color Color { get; set; }
    protected string AccentColorCssClass { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (Key == null)
            Key = KeyFactory.Empty(typeof(Category));

        if (!Key.IsEmpty)
        {
            Name = await Queries.QueryAsync(new GetCategoryName(Key));
            Color = await Queries.QueryAsync(new GetCategoryColor(Key));
            AccentColorCssClass = Color.SelectAccent("back-dark", "back-light");
        }
    }
}
