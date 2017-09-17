using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public interface IDevelopmentService
    {
        Task RebuildReadModelsAsync();

        bool IsTestDatabaseEnabled();
        bool IsMobileDevice();

        IDevelopmentService IsTestDatabaseEnabled(bool isEnabled);
        IDevelopmentService IsMobileDevice(bool isMobile);
    }
}
