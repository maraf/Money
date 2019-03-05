using Microsoft.AspNetCore.Blazor.Components;
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

        protected string UserName { get; set; }
        protected string Password { get; set; }
        protected bool IsPermanent { get; set; }

        protected bool IsError { get; set; }

        protected Task OnSubmitAsync()
            => LoginAsync(UserName, Password, IsPermanent);

        protected Task OnDemoSubmitAsync()
            => LoginAsync("demo", "demo", false);

        private async Task LoginAsync(string userName, string password, bool isPermanent)
        {
            IsError = false;

            string token = await ApiClient.LoginAsync(userName, password, isPermanent);
            if (string.IsNullOrEmpty(token))
            {
                IsError = true;
                return;
            }
        }
    }
}
