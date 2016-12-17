using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Migrations
{
    /// <summary>
    /// A context of upgrade operation.
    /// </summary>
    public interface IUpgradeContext
    {
        /// <summary>
        /// Sets a number of steps needed to execute.
        /// </summary>
        /// <param name="count"></param>
        /// <returns>Self (for fluency).</returns>
        IUpgradeContext TotalSteps(int count);

        /// <summary>
        /// Sets a currently executing step.
        /// </summary>
        /// <param name="index">An index of executing step.</param>
        /// <param name="caption">A step caption.</param>
        /// <returns>Self (for fluency).</returns>
        IUpgradeContext StartingStep(int index, string caption);
    }
}
