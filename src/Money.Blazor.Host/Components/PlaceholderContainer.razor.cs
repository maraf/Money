using Microsoft.AspNetCore.Components;

namespace Money.Components;

partial class PlaceholderContainer
{
    private static readonly PlaceholderContext activeContext = new PlaceholderContext("placeholder-glow", true);
    private static readonly PlaceholderContext inactiveContext = new PlaceholderContext(string.Empty, false);

    [Parameter]
    public bool IsActive { get; set; }

    [Parameter]
    public RenderFragment<PlaceholderContext> ChildContent { get; set; }
}