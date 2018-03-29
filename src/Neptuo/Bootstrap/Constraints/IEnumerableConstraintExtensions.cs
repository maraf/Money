using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Constraints
{
    internal static class IEnumerableConstraintExtensions
    {
        /// <summary>
        /// Evaluates all constraints in <paramref name="constraints"/>.
        /// If any constraint will not satisfies, returns <c>false</c>; if all satisfies, returns <c>true</c>.
        /// </summary>
        /// <param name="constraints">Constraints to evaluate.</param>
        /// <param name="bootstrapTask">Task, where <paramref name="constraints"/> was defined.</param>
        /// <param name="context">Context of evaluation.</param>
        /// <returns>If any constraint will not satisfies, returns <c>false</c>; if all satisfies, returns <c>true</c>.</returns>
        public static bool IsSatisfied(this IEnumerable<IBootstrapConstraint> constraints, IBootstrapTask bootstrapTask, IBootstrapConstraintContext context)
        {
            foreach (IBootstrapConstraint constraint in constraints)
            {
                if (!constraint.IsSatisfied(bootstrapTask, context))
                    return false;
            }

            return true;
        }
    }
}
