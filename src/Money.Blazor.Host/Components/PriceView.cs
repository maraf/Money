using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Money.Services;

namespace Money.Components;

public class PriceView(CurrencyFormatterFactory formatterFactory) : ComponentBase
{
    private CurrencyFormatter formatter;

    [Parameter]
    public Price Value { get; set; }

    [Parameter]
    public CurrencyFormatter.FormatZero Zero { get; set; } = CurrencyFormatter.FormatZero.Zero;

    [Parameter]
    public bool ApplyUserDigits { get; set; } = true;

    [Parameter]
    public bool ApplyPlusForPositiveNumbers { get; set; } = false;

    [Parameter]
    public string TagName { get; set; } = "span";

    [Parameter]
    public string CssClass { get; set; }

    [Parameter]
    public string CssStyle { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        formatter = await formatterFactory.CreateAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        int index = 0;
        builder.OpenElement(index++, TagName);
        builder.AddAttribute(index++, "class", $"{CssClass} text-nowrap");
        
        if (!String.IsNullOrEmpty(CssStyle))
            builder.AddAttribute(index++, "style", CssStyle);

        if (formatter != null)
            builder.AddContent(index++, formatter.Format(Value, Zero, ApplyUserDigits, ApplyPlusForPositiveNumbers));
        
        builder.CloseElement();
    }
}