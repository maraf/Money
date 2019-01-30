using Neptuo.Migrations;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.DesignData
{
    internal class MockUpgradeService : IUpgradeService
    {
        public event Func<Task> Completed;

        public bool IsRequired()
        {
            return true;
        }

        public Task UpgradeAsync(IUpgradeContext context)
        {
            context
                .TotalSteps(5)
                .StartingStep(2, "Upgrading internal database schema.");

            return Async.CompletedTask;
        }
    }
}
