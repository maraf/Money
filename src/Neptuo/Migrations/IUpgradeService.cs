using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Migrations
{
    public interface IUpgradeService
    {
        /// <summary>
        /// An event raised when upgrade is completed.
        /// </summary>
        event Func<Task> Completed;

        /// <summary>
        /// Returns a <c>true</c> if upgrade is required.
        /// </summary>
        /// <returns><c>true</c> if upgrade is required; <c>false</c> otherwise.</returns>
        bool IsRequired();

        /// <summary>
        /// Executes required upgrades.
        /// </summary>
        /// <param name="context">A context of upgrade.</param>
        /// <returns>Continuation task.</returns>
        Task UpgradeAsync(IUpgradeContext context);
    }
}
