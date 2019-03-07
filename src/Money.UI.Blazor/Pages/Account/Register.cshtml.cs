using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public class RegisterBase : BlazorComponent
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public List<string> ErrorMessages { get; } = new List<string>();

        protected Task OnSubmitAsync()
        {
            if (Validate())
            {
                UserName = null;
                Password = null;
                ConfirmPassword = null;

                ErrorMessages.Add("Passed ;-)");
            }

            return Task.CompletedTask;
        }

        protected bool Validate()
        {
            ErrorMessages.Clear();

            if (String.IsNullOrEmpty(UserName))
                ErrorMessages.Add("Please, fill user name.");

            if (String.IsNullOrEmpty(Password))
                ErrorMessages.Add("Please, fill password.");

            if (String.IsNullOrEmpty(ConfirmPassword))
                ErrorMessages.Add("Please, fill password confirmation.");
            else if (Password != ConfirmPassword)
                ErrorMessages.Add("Password must match password confirmation.");

            return ErrorMessages.Count == 0;
        }
    }
}
