using Neptuo.Bootstrap.Dependencies.Providers.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Dependencies.Providers
{
    public interface IDependencyExportDescriptor
    {
        IDependencyTarget Target { get; }

        object GetValue(IBootstrapTask task);
    }
}
