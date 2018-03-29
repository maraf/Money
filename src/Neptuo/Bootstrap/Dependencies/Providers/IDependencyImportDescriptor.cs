using Neptuo.Bootstrap.Dependencies.Providers.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Dependencies.Providers
{
    public interface IDependencyImportDescriptor
    {
        IDependencyTarget Target { get; }

        void SetValue(IBootstrapTask task, object dependency);
    }
}
