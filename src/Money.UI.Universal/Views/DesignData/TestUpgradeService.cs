using Neptuo.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.DesignData
{
    internal class TestUpgradeService : IUpgradeService
    {
        public event Func<Task> Completed;

        public bool IsRequired()
        {
            return true;
        }

        public async Task UpgradeAsync(IUpgradeContext context)
        {
            context.TotalSteps(6);
            await Task.Delay(500);

            context.StartingStep(0, "Transfering money.");
            await Task.Delay(3000);
            context.StartingStep(1, "Establishing connection to the server.");
            await Task.Delay(1000);
            context.StartingStep(2, "Rebuilding internal database.");
            await Task.Delay(8000);
            context.StartingStep(3, "Computing sums.");
            await Task.Delay(1000);
            context.StartingStep(4, "Checking internet connection.");
            await Task.Delay(1000);
            context.StartingStep(5, "Uploading to the cloud.");
            await Task.Delay(6000);
        }
    }
}
