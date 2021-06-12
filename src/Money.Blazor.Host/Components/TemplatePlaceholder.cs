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
    public class TemplatePlaceholder : ComponentBase, IDisposable
    {
        private RenderFragment content;

        [Inject]
        protected TemplateService Service { get; set; }

        [Parameter]
        public string Name { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Service.DeclarePlaceholder(Name, this);
        }

        public void Dispose()
        {
            Service.DisposePlaceholder(Name, this);
        }

        internal void UseContent(RenderFragment content)
        {
            this.content = content;
            StateHasChanged();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(0, content);
        }
    }
}
