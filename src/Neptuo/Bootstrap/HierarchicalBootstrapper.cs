using Neptuo.Bootstrap.Constraints;
using Neptuo.Bootstrap.Dependencies;
using Neptuo.Bootstrap.Dependencies.Providers;
using Neptuo.Bootstrap.Dependencies.Providers.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap
{
    /// <summary>
    /// TODO: Implement Hierarchical bootstrapper.
    /// </summary>
    /// <remarks>
    /// Řezení bootstrap tasků by mělo být takto:
    /// 1) Ti, co X exportují (vytvářejí instanci).
    /// 2) Ti, co X importují i exportují (donaplňují instanci) => co atribut [Building]?
    /// 3) Ti, co jen importují (má se předávat do kontextu/env/container).
    /// </remarks>
    public class HierarchicalBootstrapper : BootstrapperBase, IBootstrapper, IBootstrapTaskRegistry
    {
        private readonly HierarchicalContext context;
        private readonly Stack<BootstrapTaskDescriptor> currentDescriptors = new Stack<BootstrapTaskDescriptor>();
        private readonly List<BootstrapTaskDescriptor> descriptors = new List<BootstrapTaskDescriptor>();

        public HierarchicalBootstrapper(HierarchicalContext context)
            : base(context.Activator, context.ConstraintProvider)
        {
            Ensure.NotNull(context, "context");
            this.context = context;
        }

        public void Register(IBootstrapTask task)
        {
            Ensure.NotNull(task, "task");
            BootstrapTaskDescriptor descriptor = new BootstrapTaskDescriptor(task.GetType());
            descriptor.Imports.AddRange(context.DescriptorProvider.GetImports(descriptor.Type));
            descriptor.Exports.AddRange(context.DescriptorProvider.GetExports(descriptor.Type));
            descriptor.Instance = task;
            descriptors.Add(descriptor);
        }

        public void Register<T>() 
            where T : IBootstrapTask
        {
            BootstrapTaskDescriptor descriptor = new BootstrapTaskDescriptor(typeof(T));
            descriptor.Imports.AddRange(context.DescriptorProvider.GetImports(descriptor.Type));
            descriptor.Exports.AddRange(context.DescriptorProvider.GetExports(descriptor.Type));
            descriptor.Instance = CreateInstance<T>();
            descriptors.Add(descriptor);
        }

        public override void Initialize()
        {
            foreach (BootstrapTaskDescriptor descriptor in descriptors)
                ExecuteDescriptor(descriptor);
        }

        private void ExecuteDescriptor(BootstrapTaskDescriptor descriptor)
        {
            if (descriptor.IsExecuted)
                return;

            // Check dependency cycles! Use stack and check if descriptor is not present on the stack.
            if (currentDescriptors.Contains(descriptor))
                throw Ensure.Exception.InvalidOperation("Found cyclic dependency during bootstrapping of task '{0}'.", descriptor.Type.FullName);

            currentDescriptors.Push(descriptor);

            // Import.
            ProcessImports(descriptor);

            // Check constraints.
            if (AreConstraintsSatisfied(descriptor.Instance))
            {
                // Run task.
                descriptor.Instance.Initialize();

                // Export.
                ProcessExports(descriptor, descriptor.Instance);
            }

            // Mark as exported.
            descriptor.IsExecuted = true;
            currentDescriptors.Pop();
        }

        private void ProcessImports(BootstrapTaskDescriptor descriptor)
        {
            foreach (IDependencyImportDescriptor importDescriptor in descriptor.Imports)
            {
                Tuple<IDependencyExportDescriptor, BootstrapTaskDescriptor> exportTuple = FindExportDescriptor(importDescriptor.Target);
                if(exportTuple == null)
                    throw Ensure.Exception.InvalidOperation("Missing import for type '{0}', implement factory or environment import.", importDescriptor.Target.Type.FullName);

                if (!exportTuple.Item2.IsExecuted)
                    ExecuteDescriptor(exportTuple.Item2);

                importDescriptor.SetValue(descriptor.Instance, exportTuple.Item1.GetValue(exportTuple.Item2.Instance));
            }
        }

        private void ProcessExports(BootstrapTaskDescriptor descriptor, IBootstrapTask task)
        {
            foreach (IDependencyExportDescriptor exportDescriptor in descriptor.Exports)
                context.DependencyExporter.Export(exportDescriptor, exportDescriptor.GetValue(task));
        }

        private Tuple<IDependencyExportDescriptor, BootstrapTaskDescriptor> FindExportDescriptor(IDependencyTarget target)
        {
            foreach (BootstrapTaskDescriptor descriptor in descriptors)
            {
                foreach (IDependencyExportDescriptor exportDescriptor in descriptor.Exports)
                {
                    if (exportDescriptor.Target.Equals(target))
                        return new Tuple<IDependencyExportDescriptor,BootstrapTaskDescriptor>(exportDescriptor, descriptor);
                }
            }

            return null;
        }


        //void ITaskExecutor.Execute(IBootstrapTask task)
        //{
        //    Ensure.NotNull(task, "task");
        //    BootstrapTaskDescriptor descriptor = descriptors.FirstOrDefault(d => d.Instance == task);
        //    if (descriptor == null)
        //    {
        //        Type taskType = task.GetType();
        //        descriptor = descriptors.FirstOrDefault(d => d.Type == taskType);

        //        if (descriptor == null)
        //            descriptors.Add(descriptor = new BootstrapTaskDescriptor(taskType) { Instance = task });
        //    }

        //    ExecuteDescriptor(descriptor);
        //}





        private class BootstrapTaskDescriptor
        {
            public Type Type { get; private set; }
            public IBootstrapTask Instance { get; set; }

            public List<IDependencyImportDescriptor> Imports { get; private set; }
            public List<IDependencyExportDescriptor> Exports { get; private set; }

            public bool IsExecuted { get; set; }

            public BootstrapTaskDescriptor(Type type)
            {
                Ensure.NotNull(type, "type");
                Type = type;
                Imports = new List<IDependencyImportDescriptor>();
                Exports = new List<IDependencyExportDescriptor>();
            }
        }
    }
}
