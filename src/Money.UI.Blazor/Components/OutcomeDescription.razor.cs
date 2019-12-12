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
    public partial class OutcomeDescription
    {
        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        private string originalDescription;
        protected List<string> ErrorMessages { get; } = new List<string>();

        [Parameter]
        public IKey OutcomeKey { get; set; }

        [Parameter]
        public string Description { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            originalDescription = Description;
        }

        protected void OnSaveClick()
        {
            if (Validate() && originalDescription != Description)
            {
                Execute();
                OnParametersSet();
                Modal.Hide();
            }
        }

        private bool Validate()
        {
            ErrorMessages.Clear();
            Validator.AddOutcomeDescription(ErrorMessages, Description);

            return ErrorMessages.Count == 0;
        }

        private async void Execute()
            => await Commands.HandleAsync(new ChangeOutcomeDescription(OutcomeKey, Description));
    }
}
