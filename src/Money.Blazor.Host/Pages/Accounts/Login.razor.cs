using Microsoft.AspNetCore.Components;
using Money.Models.Loading;
using Money.Services;
using Neptuo.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages.Accounts
{
    public partial class Login
    {
        [Inject]
        internal Navigator Navigator { get; set; }

        [Inject]
        internal ApiClient ApiClient { get; set; }

        [Inject]
        internal QueryString QueryString { get; set; }

        [Inject]
        internal TokenContainer Token { get; set; }

        [Parameter]
        public string ReturnUrl { get; set; }

        protected string UserName { get; set; }
        protected string Password { get; set; }
        protected bool IsPermanent { get; set; }

        protected LoadingContext Loading { get; } = new LoadingContext();
        protected List<string> ErrorMessages { get; } = new List<string>();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (Token.HasValue)
                NavigateAway();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ReturnUrl = QueryString.Find<string>("returnUrl");
        }

        protected Task OnSubmitAsync()
            => LoginAsync(UserName, Password, IsPermanent);

        protected Task OnDemoSubmitAsync()
            => LoginAsync("demo", "demo", false);

        private async Task LoginAsync(string userName, string password, bool isPermanent)
        {
            using (Loading.Start())
            {
                ErrorMessages.Clear();

                if (Validate(userName, password))
                {
                    if (!await ApiClient.LoginAsync(userName, password, isPermanent))
                        ErrorMessages.Add("User name and password don't match.");
                    else
                        NavigateAway();
                }
            }
        }

        private void NavigateAway()
        {
            if (!String.IsNullOrEmpty(ReturnUrl))
                Navigator.Open(ReturnUrl);
            else if (Navigator.IsLoginUrl())
                Navigator.OpenSummary();
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
