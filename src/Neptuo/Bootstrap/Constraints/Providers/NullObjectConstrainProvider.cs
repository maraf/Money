using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Constraints.Providers
{
    /// <summary>
    /// Empty implementation of <see cref="IBootstrapConstraintProvider"/>.
    /// </summary>
    internal class NullObjectConstrainProvider : IBootstrapConstraintProvider
    {
        private List<IBootstrapConstraint> result = new List<IBootstrapConstraint>();

        public IEnumerable<IBootstrapConstraint> GetConstraints(Type bootstrapTask)
        {
            return result;
        }
    }
}
