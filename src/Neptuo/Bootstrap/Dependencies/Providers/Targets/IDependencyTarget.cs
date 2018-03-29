using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Dependencies.Providers.Targets
{
    public interface IDependencyTarget : IEquatable<IDependencyTarget>
    {
        Type Type { get; }
    }
}
