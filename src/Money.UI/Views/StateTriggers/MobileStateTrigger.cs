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
        public MobileStateTrigger()
        {
            string deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
            bool isActive = deviceFamily == "Windows.Mobile";
            SetActive(isActive);
        }
    }
}
