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
        /// The implementation of <see cref="IDateTimeProvider"/> which uses <see cref="DateTime.Now"/>.
        /// </summary>
        public class DateTimeNowProvider : IDateTimeProvider
        {
            public TimeSpan GetExecutionDelay(DateTime dateTime)
            {
                return dateTime.Subtract(DateTime.Now);
            }
        }
    }
}
