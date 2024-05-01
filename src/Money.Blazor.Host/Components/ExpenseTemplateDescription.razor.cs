using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components.Bootstrap;
using Money.Services;
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
    public partial class ExpenseTemplateDescription
    {
        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        private string originalDescription;

        [Parameter]
        public IKey ExpenseTemplateKey { get; set; }

        [Parameter]
        public string Description { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            originalDescription = Description;
        }

        protected void OnSaveClick()
        {
            if (originalDescription != Description)
            {
                Execute();
                OnParametersSet();
                Modal.Hide();
            }
        }

        private async void Execute()
            => await Commands.HandleAsync(new ChangeExpenseTemplateDescription(ExpenseTemplateKey, Description));
    }
}
