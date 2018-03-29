using Neptuo.Bootstrap.Constraints;
using Neptuo.Bootstrap.Constraints.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap
{
    public abstract class BootstrapperBase : IBootstrapper
    {
        private IBootstrapConstraintProvider provider;
        private Func<Type, IBootstrapTask> factory;

        protected List<IBootstrapTask> Tasks { get; private set; }

        public BootstrapperBase(Func<Type, IBootstrapTask> factory, IBootstrapConstraintProvider provider = null)
        {
            Ensure.NotNull(factory, "factory");
            this.factory = factory;
            this.provider = provider ?? new NullObjectConstrainProvider();
            Tasks = new List<IBootstrapTask>();
        }

        protected IBootstrapTask CreateInstance(Type type)
        {
            return factory(type);
        }

        protected IBootstrapTask CreateInstance<T>()
            where T : IBootstrapTask
        {
            return factory(typeof(T));
        }

        protected bool AreConstraintsSatisfied(IBootstrapTask task)
        {
            IBootstrapConstraintContext context = new DefaultBootstrapConstraintContext(this);
            return provider.GetConstraints(task.GetType()).IsSatisfied(task, context);
        }

        public virtual void Initialize()
        {
            IBootstrapConstraintContext context = new DefaultBootstrapConstraintContext(this);
            foreach (IBootstrapTask task in Tasks)
            {
                if (provider.GetConstraints(task.GetType()).IsSatisfied(task, context))
                    InitializeTask(task);
            }
        }

        protected virtual void InitializeTask(IBootstrapTask task)
        {
            task.Initialize();
        }
    }
}
