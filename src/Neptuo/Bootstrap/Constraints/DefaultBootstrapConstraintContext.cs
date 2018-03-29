using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Constraints
{
    /// <summary>
    /// Default implementation of <see cref="IBootstrapConstraintContext"/>.
    /// </summary>
    internal class DefaultBootstrapConstraintContext : IBootstrapConstraintContext
    {
        public IBootstrapper Bootstrapper { get; private set; }

        public DefaultBootstrapConstraintContext(IBootstrapper bootstrapper)
        {
            Ensure.NotNull(bootstrapper, "bootstrapper");
            Bootstrapper = bootstrapper;
        }
    }
}
