using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Constraints
{
    /// <summary>
    /// Provides constraints for task class.
    /// </summary>
    public interface IBootstrapConstraintProvider
    {
        /// <summary>
        /// Provides enumeration of constraints for the <paramref name="bootstrapTask"/>.
        /// </summary>
        /// <param name="bootstrapTask">Type of bootstrap task.</param>
        /// <returns>Enumeration of constraints for the <paramref name="bootstrapTask"/>.</returns>
        IEnumerable<IBootstrapConstraint> GetConstraints(Type bootstrapTask);
    }
}
