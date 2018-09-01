using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

namespace Money.Services
{
    public class RestartService
    {
        private readonly string launchArguments;

        public RestartService(string launchArguments)
        {
            this.launchArguments = launchArguments;
        }

        public async Task RestartAsync()
        {
            AppRestartFailureReason result = await CoreApplication.RequestRestartAsync(launchArguments);
            if (result == AppRestartFailureReason.NotInForeground || result == AppRestartFailureReason.Other)
                Application.Current.Exit();
        }
    }
}
