using Microsoft.JSInterop;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components;

public class AutoloadNextInterop(IJSRuntime jsRuntime)
{
    private AutoloadNext component;

    public async Task InitializedAsync(AutoloadNext component)
    {
        Ensure.NotNull(component, "component");
        this.component = component;

        await jsRuntime.InvokeVoidAsync("AutoloadNext.Initialize", component.Container, DotNetObjectReference.Create(this));
    }

    [JSInvokable("AutoloadNext.Intersected")]
    public void Intersected() 
        => _ = component.Intersected.InvokeAsync();
}
