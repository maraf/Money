using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap
{
    public interface IBootstrapTaskRegistry
    {
        void Register(IBootstrapTask task);
        void Register<T>() where T : IBootstrapTask;
    }
}
