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
    public partial class CategoryColor
    {
        private Color? originalColor;

        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Inject]
        protected ColorCollection Colors { get; set; }

        [Parameter]
        public IKey CategoryKey { get; set; }

        [Parameter]
        public Color? Color { get; set; }

        protected override void OnParametersSet()
        {
            originalColor = Color;
        }

        protected void OnSaveClick()
        {
            if (originalColor != Color)
            {
                Execute();
                Modal.Hide();
            }
        }

        private async void Execute()
        {
            if (Color != null)
            {
                await Commands.HandleAsync(new ChangeCategoryColor(CategoryKey, Color.Value));
                originalColor = Color;
            }
        }
    }
}
