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
    public class TemplateContent : ComponentBase
    {
        [Inject]
        protected TemplateService Service { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            Console.WriteLine($"TC: '{Name}'.");
            Service.UseContent(Name, ChildContent);
        }
    }
}
