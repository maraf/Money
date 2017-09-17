using Money.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;
using Windows.UI.Xaml;

namespace Money.Views.StateTriggers
{
    public class MobileStateTrigger : StateTriggerBase
    {
        private readonly IDevelopmentService developmentTools = ServiceProvider.DevelopmentTools;

        public bool IsActive { get; private set; }

        public MobileStateTrigger()
        {
            bool isActive = developmentTools.IsMobileDevice();
            if (!isActive)
            {
                string deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
                isActive = deviceFamily == "Windows.Mobile";
            }

            IsActive = isActive;
            SetActive(isActive);
        }
    }
}
