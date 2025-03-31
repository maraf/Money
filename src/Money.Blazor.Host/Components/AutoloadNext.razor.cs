using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components;

public partial class AutoloadNext(AutoloadNextInterop Interop)
{
    protected internal ElementReference Container { get; set; }

    [Parameter]
    public EventCallback Intersected { get; set; }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        await Interop.InitializedAsync(this);
    }
}
