using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components.Bootstrap;
using Money.Models;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class CategoryIcon
    {
        private string originalIcon;

        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected IconCollection Icons { get; set; }

        [Parameter]
        public IKey CategoryKey { get; set; }

        [Parameter]
        public string Icon { get; set; }

        protected override void OnParametersSet()
        {
            originalIcon = Icon;
        }

        protected void OnSaveClick()
        {
            if (originalIcon != Icon)
            {
                Execute();
                Modal.Hide();
            }
        }

        private async void Execute()
        {
            await Commands.HandleAsync(new ChangeCategoryIcon(CategoryKey, Icon));
            originalIcon = Icon;
        }
    }
}
