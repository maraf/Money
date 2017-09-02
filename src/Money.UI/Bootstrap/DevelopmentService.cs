using Money.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Bootstrap
{
    internal class DevelopmentService : IDevelopmentService
    {
        private readonly UpgradeService upgradeService;

        internal DevelopmentService(UpgradeService upgradeService)
        {
            this.upgradeService = upgradeService;
        }

        public Task RebuildReadModelsAsync()
        {
            return upgradeService.RecreateReadModelContextAsync();
        }
    }
}
