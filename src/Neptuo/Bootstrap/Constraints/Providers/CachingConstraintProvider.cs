using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Constraints.Providers
{
    /// <summary>
    /// Caches constraints for bootstrap tasks by type.
    /// </summary>
    public class CachingConstraintProvider : IBootstrapConstraintProvider
    {
        private readonly Dictionary<Type, IEnumerable<IBootstrapConstraint>> cache = new Dictionary<Type, IEnumerable<IBootstrapConstraint>>();
        private readonly IBootstrapConstraintProvider provider;

        public CachingConstraintProvider(IBootstrapConstraintProvider provider)
        {
            Ensure.NotNull(provider, "provider");
            this.provider = provider;
        }

        public IEnumerable<IBootstrapConstraint> GetConstraints(Type bootstrapTask)
        {
            if (cache.ContainsKey(bootstrapTask))
                return cache[bootstrapTask];

            IEnumerable<IBootstrapConstraint> constraints = provider.GetConstraints(bootstrapTask);
            cache[bootstrapTask] = constraints;
            return constraints;
        }

        /// <summary>
        /// Clears cache of bootstrap tasks constaints.
        /// </summary>
        public void ClearCache()
        {
            cache.Clear();
        }
    }
}
