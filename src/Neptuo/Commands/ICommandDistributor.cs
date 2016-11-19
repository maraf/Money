using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands
{
    /// <summary>
    /// Describes component that distributes command execution.
    /// </summary>
    public interface ICommandDistributor
    {
        /// <summary>
        /// Returns object, which should property implement <see cref="Object.GetHashCode"/> and <see cref="Object.Equals"/> to determine the 'queue'
        /// where the <paramref name="command"/> should be processed.
        /// 
        /// This method tells which commands belong together and so can't be processed paralelly.
        /// </summary>
        /// <param name="command">The command to assign to the 'queue'.</param>
        /// <returns></returns>
        object Distribute(object command);
    }
}
