using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging
{
    internal class DefaultLog<T> : ILog<T>
    {
        private ILog log;

        public DefaultLog(ILogFactory logFactory)
        {
            Ensure.NotNull(logFactory, "logFactory");
            log = logFactory.Scope(typeof(T).Name);
        }

        public ILogFactory Factory => log.Factory;

        public bool IsLevelEnabled(LogLevel level)
            => log.IsLevelEnabled(level);

        public void Log(LogLevel level, object model)
            => log.Log(level, model);
    }
}
