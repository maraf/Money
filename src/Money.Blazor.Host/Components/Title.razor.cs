using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class Title
    {
        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public string Main { get; set; }

        [Parameter]
        public string Sub { get; set; }

        [Parameter]
        public string ButtonIcon { get; set; } = "plus";

        [Parameter]
        public string ButtonText { get; set; }

        [Parameter]
        public Action ButtonClick { get; set; }

        [Parameter]
        public RenderFragment ButtonContent { get; set; }
    }
}
