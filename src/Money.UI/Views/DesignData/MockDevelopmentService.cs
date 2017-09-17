using Money.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.DesignData
{
    internal class MockDevelopmentService : IDevelopmentService
    {
        public bool IsMobileDevice()
        {
            return false;
        }

        public IDevelopmentService IsMobileDevice(bool isMobile)
        {
            return this;
        }

        public bool IsTestDatabaseEnabled()
        {
            return false;
        }

        public IDevelopmentService IsTestDatabaseEnabled(bool isEnabled)
        {
            return this;
        }

        public Task RebuildReadModelsAsync()
        {
            return Task.CompletedTask;
        }
    }
}
