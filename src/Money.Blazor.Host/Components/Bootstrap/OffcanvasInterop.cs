using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Neptuo;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components.Bootstrap;

public class OffcanvasInterop(IJSRuntime jsRuntime)
{
    private readonly IJSRuntime jsRuntime;
    private Offcanvas component;

    public async Task InitializeAsync(Offcanvas component, ElementReference element)
    {
        this.component = component;
        await jsRuntime.InvokeVoidAsync("Bootstrap.Offcanvas.Initialize", DotNetObjectReference.Create(this), element);
    }

    internal void Show(ElementReference element)
    {
        jsRuntime.InvokeVoidAsync("Bootstrap.Offcanvas.Show", element);
    }

    internal void Hide(ElementReference element)
    {
        jsRuntime.InvokeVoidAsync("Bootstrap.Offcanvas.Hide", element);
    }

    internal void Dispose(ElementReference element)
    {
        jsRuntime.InvokeVoidAsync("Bootstrap.Offcanvas.Dispose", element);
    }

    [JSInvokable("Offcanvas.VisibilityChanged")]
    public void VisibilityChanged(bool isVisible)
    {
        component.UpdateVisibility(isVisible);
    }
}