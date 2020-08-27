using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Layouts
{
    public partial class MainMenuSeparator
    {
        [Parameter]
        public bool IsVisibleOnDesktop { get; set; } = true;

        [Parameter]
        public bool IsVisibleOnMobile { get; set; } = true;
    }
}
