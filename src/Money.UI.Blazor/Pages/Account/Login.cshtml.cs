using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.Services;
using Money.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public class LoginBase : BlazorComponent
    {
        [Inject]
        internal Navigator Navigator { get; set; }

        [Inject]
        internal ApiClient ApiClient { get; set; }

        [Inject]
        internal IUriHelper Uri { get; set; }

        [Parameter]
        protected string ReturnUrl { get; set; }

        protected string UserName { get; set; }
        protected string Password { get; set; }
        protected bool IsPermanent { get; set; }

        protected List<string> ErrorMessages { get; } = new List<string>();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            SetReturnUrl();
        }

        private void SetReturnUrl()
        {
            string url = Uri.GetAbsoluteUri();
            int indexOfQuery = url.IndexOf('?');
            if (indexOfQuery >= 0)
            {
                string query = url.Substring(indexOfQuery + 1).ToLowerInvariant();
                string[] parameters = query.Split('&');
                foreach (string parameter in parameters)
                {
                    string[] keyValue = parameter.Split('=');
                    if (keyValue[0] == "returnurl")
                    {
                        if (keyValue.Length == 2)
                            ReturnUrl = keyValue[1];

                        break;
                    }
                }
            }
        }

        protected Task OnSubmitAsync()
            => LoginAsync(UserName, Password, IsPermanent);

        protected Task OnDemoSubmitAsync()
            => LoginAsync("demo", "demo", false);

        private async Task LoginAsync(string userName, string password, bool isPermanent)
        {
            ErrorMessages.Clear();

            if (Validate(userName, password))
            {
                if (!await ApiClient.LoginAsync(userName, password, isPermanent))
                    ErrorMessages.Add("User name and password don't match.");
                else if (ReturnUrl != null)
                    Uri.NavigateTo(ReturnUrl);
                else
                    Navigator.OpenSummary();
            }
        }

        private bool Validate(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
                ErrorMessages.Add("Please, fill user name.");

            if (String.IsNullOrEmpty(password))
                ErrorMessages.Add("Please, fill password.");

            return ErrorMessages.Count == 0;
        }
    }
}
