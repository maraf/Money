using Neptuo.Bootstrap.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap
{
    public class AutomaticBootstrapper : BootstrapperBase, IBootstrapper
    {
        private IEnumerable<Type> types;

        public AutomaticBootstrapper(Func<Type, IBootstrapTask> factory, IBootstrapConstraintProvider provider = null)
            : base(factory, provider)
        { }

        public AutomaticBootstrapper(Func<Type, IBootstrapTask> factory, IEnumerable<Type> types, IBootstrapConstraintProvider provider = null)
            : base(factory, provider)
        {
            this.types = AddSupportedTypes(new List<Type>(), types);
        }

        public override void Initialize()
        {
            if (types == null)
                types = FindTypes();

            foreach (Type type in types)
            {
                IBootstrapTask instance = CreateInstance(type);
                if (AreConstraintsSatisfied(instance))
                    Tasks.Add(instance);
            }

            foreach (IBootstrapTask task in Tasks)
                task.Initialize();
        }

        protected virtual IEnumerable<Type> FindTypes()
        {
            List<Type> types = new List<Type>();
            SearchAssemblies(types);
            return types;
        }

        protected virtual IEnumerable<Type> SearchAssemblies(List<Type> types)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    AddSupportedTypes(types, assembly.GetTypes());
                }
                catch (Exception) { }
            }
            return types;
        }

        protected virtual List<Type> AddSupportedTypes(List<Type> target, IEnumerable<Type> sourceTypes)
        {
            if (target == null)
                target = new List<Type>();

            Type bootstrapInterfaceType = typeof(IBootstrapTask);
            foreach (Type type in sourceTypes)
            {
                if (bootstrapInterfaceType.IsAssignableFrom(type) && bootstrapInterfaceType != type && !type.IsAbstract && !type.IsInterface)
                    target.Add(type);
            }
            return target;
        }
    }
}
