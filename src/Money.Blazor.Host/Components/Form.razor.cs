using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Money.Components;

public partial class Form
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public EventCallback OnSubmit { get; set; }

    public bool IsSaving { get; private set; }

    public Task SubmitAsync() => OnFormSubmitAsync();

    protected async Task OnFormSubmitAsync()
    {
        if (IsSaving)
            return;

        IsSaving = true;
        StateHasChanged();

        try
        {
            await OnSubmit.InvokeAsync();
        }
        finally
        {
            IsSaving = false;
            StateHasChanged();
        }
    }
}
