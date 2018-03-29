using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap
{
    public class ManualBootstrapper : BootstrapperBase, IBootstrapper, IBootstrapTaskRegistry
    {
        public ManualBootstrapper(Func<Type, IBootstrapTask> factory)
            : base(factory)
        { }

        public virtual void Register(Type type)
        {
            Register(CreateInstance(type));
        }

        public void Register<T>()
            where T : IBootstrapTask
        {
            Register(typeof(T));
        }

        public virtual void Register(IBootstrapTask task)
        {
            if (task != null)
                Tasks.Add(task);
        }
    }
}
