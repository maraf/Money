using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public class ConvertToString<T> : ComponentBase
    {
        [Parameter]
        public T Value { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (Converts.Try(Value, out string stringValue))
                builder.AddContent(0, stringValue);
        }
    }
}
