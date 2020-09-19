using Microsoft.AspNetCore.Components;
using Money.Models.Loading;
using Money.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages.Accounts
{
    public partial class Register
    {
        [Inject]
        internal ApiClient ApiClient { get; set; }

        [Inject]
        internal Navigator Navigator { get; set; }

        [Inject]
        internal TokenContainer Token { get; set; }

        protected string UserName { get; set; }
        protected string Password { get; set; }
        protected string ConfirmPassword { get; set; }

        protected LoadingContext Loading { get; } = new LoadingContext();
        protected List<string> ErrorMessages { get; } = new List<string>();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (Token.HasValue)
                Navigator.OpenSummary();
        }

        protected async Task OnSubmitAsync()
        {
            if (Validate())
            {
                using (Loading.Start())
                {
                    var response = await ApiClient.RegisterAsync(UserName, Password);
                    if (response.IsSuccess)
                    {
                        if (await ApiClient.LoginAsync(UserName, Password, false))
                        {
                            Navigator.OpenSummary();
                        }
                        else
                        {
                            UserName = null;
                            Password = null;
                            ConfirmPassword = null;

                            Navigator.OpenLogin();
                        }
                    }
                    else
                    {
                        ErrorMessages.AddRange(response.ErrorMessages);
                    } 
                }
            }
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
                ErrorMessages.Add("Passwords must match.");

            return ErrorMessages.Count == 0;
        }
    }
}
