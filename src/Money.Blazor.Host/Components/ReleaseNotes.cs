using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Money.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class ReleaseNotes : ComponentBase
    {
        [Inject]
        protected Navigator Navigator { get; set; }

        private readonly HttpClient http = new HttpClient();
        private static Task<string> getTask;
        private string value;

        protected override async Task OnInitializedAsync()
        {
            http.BaseAddress = new Uri(Navigator.UrlOrigin());

            await base.OnInitializedAsync();

            if (getTask == null)
                getTask = http.GetStringAsync("/release-notes.html");

            value = await getTask;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.AddMarkupContent(0, value);
        }
    }
}
