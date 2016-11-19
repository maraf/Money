using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    partial class TimerSchedulingProvider
    {
		/// <summary>
        /// The provider of difference between scheduled datetime and
        /// </summary>
		public interface IDateTimeProvider
        {
            /// <summary>
            /// Computes a delay after which a command or an event should be executed.
            /// </summary>
            /// <param name="executeAt">The date and time when execution should be done.</param>
            /// <returns>A delay after which a command or an event should be executed.</returns>
            TimeSpan GetExecutionDelay(DateTime dateTime);
        }
    }
}
