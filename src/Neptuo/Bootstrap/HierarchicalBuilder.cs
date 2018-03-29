using Neptuo.Activators;
using Neptuo.Bootstrap.Constraints;
using Neptuo.Bootstrap.Constraints.Providers;
using Neptuo.Bootstrap.Dependencies.Providers;
using Neptuo.Bootstrap.Dependencies.Providers.Exporters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap
{
    public class HierarchicalBuilder : IHierarchicalBuilderActivator, IHierarchicalBuilderConstraint, IHierarchicalBuilderDescriptor, IHierarchicalBuilderExporter
    {
        private Func<Type, IBootstrapTask> activator;
        private IBootstrapConstraintProvider constraintProvider;
        private IDependencyDescriptorProvider descriptorProvider;

        #region IHierarchicalBuilderActivator

        public IHierarchicalBuilderConstraint WithActivator(IDependencyProvider dependencyProvider)
        {
            Ensure.NotNull(dependencyProvider, "dependencyProvider");
            activator = type => (IBootstrapTask)dependencyProvider.Resolve(type);
            return this;
        }

        public IHierarchicalBuilderConstraint WithActivator(Func<Type, IBootstrapTask> activator)
        {
            Ensure.NotNull(activator, "activator");
            this.activator = activator;
            return this;
        }

        public IHierarchicalBuilderConstraint WithSystemActivator()
        {
            activator = type => (IBootstrapTask)Activator.CreateInstance(type);
            return this;
        }

        #endregion

        #region IHierarchicalBuilderDescriptor

        IHierarchicalBuilderDescriptor IHierarchicalBuilderConstraint.WithConstraintProvider(IBootstrapConstraintProvider constraintProvider)
        {
            Ensure.NotNull(constraintProvider, "constraintProvider");
            this.constraintProvider = constraintProvider;
            return this;
        }

        IHierarchicalBuilderDescriptor IHierarchicalBuilderConstraint.WithoutConstraintProvider()
        {
            constraintProvider = new NullObjectConstrainProvider();
            return this;
        }

        #endregion

        #region IHierarchicalBuilderDescriptor

        IHierarchicalBuilderExporter IHierarchicalBuilderDescriptor.WithDescriptorProvider(IDependencyDescriptorProvider descriptorProvider)
        {
            Ensure.NotNull(descriptorProvider, "descriptorProvider");
            this.descriptorProvider = descriptorProvider;
            return this;
        }

        IHierarchicalBuilderExporter IHierarchicalBuilderDescriptor.WithPropertyDescriptorProvider()
        {
            descriptorProvider = new PropertyDescriptorProvider();
            return this;
        }

        #endregion

        #region IHierarchicalBuilderExporter

        HierarchicalBootstrapper IHierarchicalBuilderExporter.WithExporter(IDependencyExporter dependencyExporter)
        {
            Ensure.NotNull(dependencyExporter, "export");
            return CreateBootstrapper(dependencyExporter);
        }

        HierarchicalBootstrapper IHierarchicalBuilderExporter.WithEnvironmentExporter()
        {
            throw new NotImplementedException();
        }

        private HierarchicalBootstrapper CreateBootstrapper(IDependencyExporter dependencyExporter)
        {
            return new HierarchicalBootstrapper(new HierarchicalContext(activator, constraintProvider, descriptorProvider, dependencyExporter));
        }

        #endregion
    }

    /// <summary>
    /// For preparing bootstrap task factories.
    /// </summary>
    public interface IHierarchicalBuilderActivator
    {
        IHierarchicalBuilderConstraint WithActivator(IDependencyProvider dependencyProvider);
        IHierarchicalBuilderConstraint WithActivator(Func<Type, IBootstrapTask> activator);
        IHierarchicalBuilderConstraint WithSystemActivator();
    }

    /// <summary>
    /// For preparing constraint providers.
    /// </summary>
    public interface IHierarchicalBuilderConstraint
    {
        IHierarchicalBuilderDescriptor WithConstraintProvider(IBootstrapConstraintProvider constraintProvider);
        IHierarchicalBuilderDescriptor WithoutConstraintProvider();

    }

    /// <summary>
    /// For preparing descriptor providers.
    /// </summary>
    public interface IHierarchicalBuilderDescriptor
    {
        IHierarchicalBuilderExporter WithDescriptorProvider(IDependencyDescriptorProvider descriptorProvider);
        IHierarchicalBuilderExporter WithPropertyDescriptorProvider();
    }

    /// <summary>
    /// For preparing dependency exporters and creating bootstrappers.
    /// </summary>
    public interface IHierarchicalBuilderExporter
    {
        HierarchicalBootstrapper WithExporter(IDependencyExporter dependencyExporter);
        HierarchicalBootstrapper WithEnvironmentExporter();
    }
}
