using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Threading.Tasks
{
    /// <summary>
    /// Task extensions and utilities.
    /// </summary>
    public static class Async
    {
        private static readonly Task completedTask = Task.FromResult(true);

        /// <summary>
        /// Returns completed task instance.
        /// </summary>
        public static Task CompletedTask
        {
            get { return completedTask; }
        }
    }
}
