using Microsoft.AspNetCore.Components;
using Money.Models.Loading;
using System.Collections.Generic;

namespace Money.Components;

public partial class AutoLoadListView<TModel>
{
    [Parameter]
    public List<TModel> Items { get; set; }

    [Parameter]
    public int? PlaceholderCount { get; set; }

    [Parameter]
    public RenderFragment<ItemContext> ChildContent { get; set; }

    [Parameter]
    public PagingContext PagingContext { get; set; }

    [Parameter]
    public LoadingContext LoadingContext { get; set; }

    [Parameter]
    public string NoDataMessage { get; set; }

    public record ItemContext(TModel Model, PlaceholderContext Placeholder);
}

