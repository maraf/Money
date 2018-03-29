using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Constraints
{
    /// <summary>
    /// Describes context of constraint evaluation.
    /// </summary>
    public interface IBootstrapConstraintContext
    {
        /// <summary>
        /// Current bootstrapper.
        /// </summary>
        IBootstrapper Bootstrapper { get; }
    }
}
