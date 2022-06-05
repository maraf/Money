using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components;
using Money.Components.Bootstrap;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Pages.Users;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components.Settings
{
    public partial class PropertyDialog
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public PropertyViewModel Model { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected Modal Editor { get; set; }

        public void Show() => Editor.Show();
    }
}