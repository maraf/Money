using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public class Version : ComponentBase
    {
        [Parameter]
        public System.Version Value { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (Value != null)
            {
                string text = Value.Revision == 0 
                    ? Value.ToString(3) 
                    : Value.ToString(4);

                builder.AddContent(0, $"v{text}");
            }
        }
    }
}
