using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Constraints
{
    /// <summary>
    /// Constraint for task execution.
    /// </summary>
    public interface IBootstrapConstraint
    {
        /// <summary>
        /// Descides whether task can be executed in context of <paramref name="context"/>.
        /// </summary>
        /// <param name="task">Task to executed.</param>
        /// <param name="context">Context of task of constraint evaluation.</param>
        /// <returns><c>true</c> if <paramref name="task"/> can be executed; <c>false</c> otherwise.</returns>
        bool IsSatisfied(IBootstrapTask task, IBootstrapConstraintContext context);
    }
}
