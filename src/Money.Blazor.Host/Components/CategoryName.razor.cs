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
    public partial class CategoryName
    {
        private string originalName;
        private string originalDescription;
        protected List<string> ErrorMessages { get; } = new List<string>();

        [Inject]
        protected ICommandDispatcher Commands { get; set; }

        [Parameter]
        public IKey CategoryKey { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Description { get; set; } = "";

        protected string Title { get; set; }
        protected string SaveButtonText { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (CategoryKey == null || CategoryKey.IsEmpty)
            {
                Title = "Create a new Category";
                SaveButtonText = "Create";
            }
            else
            {
                Title = "Rename Category";
                SaveButtonText = "Save";
            }

            originalName = Name;
            originalDescription = Description;
        }

        protected void OnSaveClick()
        {
            if (Description == null)
                Description = String.Empty;

            if (CategoryKey == null || CategoryKey.IsEmpty)
            {
                if (Validate())
                {
                    ExecuteCreate();
                    Modal.Hide();
                }
            }
            else
            {
                if (Validate())
                {
                    if (originalName != Name)
                        ExecuteRename();

                    if (originalDescription != Description)
                        ExecuteChangeDescription();

                    Modal.Hide();
                }
            }
        }

        private bool Validate()
        {
            ErrorMessages.Clear();
            Validator.AddCategoryName(ErrorMessages, Name);

            return ErrorMessages.Count == 0;
        }

        private async void ExecuteCreate()
        {
            await Commands.HandleAsync(new CreateCategory(Name, Description, NextRandomColor()));
            Name = Description = String.Empty;
            StateHasChanged(); ;
        }

        private async void ExecuteRename()
        {
            await Commands.HandleAsync(new RenameCategory(CategoryKey, Name));
            originalName = Name;
        }

        private async void ExecuteChangeDescription()
        {
            await Commands.HandleAsync(new ChangeCategoryDescription(CategoryKey, Description));
            originalDescription = Description;
        }

        private Random rnd = new Random();

        protected Color NextRandomColor()
            => Color.FromArgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
    }
}
