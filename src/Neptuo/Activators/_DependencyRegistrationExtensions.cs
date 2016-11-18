using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// Common extensions for registering type to <see cref="IDependencyDefinitionCollection"/>.
    /// </summary>
    public static class _DependencyRegistrationExtensions
    {
        #region AddTransient

        public static IDependencyDefinitionCollection AddTransient<TInterface, TImplementation>(this IDependencyDefinitionCollection collection)
            where TImplementation : TInterface
        {
            Ensure.NotNull(collection, "collection");
            collection.Add(typeof(TInterface), DependencyLifetime.Transient, typeof(TImplementation));
            return collection;
        }

        public static IDependencyDefinitionCollection AddTransient<TImplementation>(this IDependencyDefinitionCollection collection)
        {
            Ensure.NotNull(collection, "collection");
            collection.Add(typeof(TImplementation), DependencyLifetime.Transient, typeof(TImplementation));
            return collection;
        }

        public static IDependencyDefinitionCollection AddTransientFactory<TInterface, TFactory>(this IDependencyDefinitionCollection collection, TFactory factory)
            where TFactory : IFactory<TInterface>
        {
            Ensure.NotNull(collection, "collection");
            collection.Add(typeof(TInterface), DependencyLifetime.Transient, factory);
            return collection;
        }

        public static IDependencyDefinitionCollection AddTransientFactory<TInterface, TFactory>(this IDependencyDefinitionCollection collection)
            where TFactory : IFactory<TInterface>
        {
            Ensure.NotNull(collection, "collection");
            collection.Add(typeof(TInterface), DependencyLifetime.Transient, typeof(TFactory));
            return collection;
        }

        #endregion

        #region AddScoped

        public static IDependencyDefinitionCollection AddScoped<TInterface, TImplementation>(this IDependencyDefinitionCollection collection)
            where TImplementation : TInterface
        {
            Ensure.NotNull(collection, "collection");
            collection.Add(typeof(TInterface), DependencyLifetime.Scope, typeof(TImplementation));
            return collection;
        }

        public static IDependencyDefinitionCollection AddScoped<TImplementation>(this IDependencyDefinitionCollection collection)
        {
            Ensure.NotNull(collection, "collection");
            collection.Add(typeof(TImplementation), DependencyLifetime.Scope, typeof(TImplementation));
            return collection;
        }

        public static IDependencyDefinitionCollection AddScopedFactory<TInterface, TFactory>(this IDependencyDefinitionCollection collection, TFactory factory)
            where TFactory : IFactory<TInterface>
        {
            Ensure.NotNull(collection, "collection");
            collection.Add(typeof(TInterface), DependencyLifetime.Scope, factory);
            return collection;
        }

        public static IDependencyDefinitionCollection AddScopedFactory<TInterface, TFactory>(this IDependencyDefinitionCollection collection)
            where TFactory : IFactory<TInterface>
        {
            Ensure.NotNull(collection, "collection");
            collection.Add(typeof(TInterface), DependencyLifetime.Scope, typeof(TFactory));
            return collection;
        }

        #endregion

        #region AddScoped

        public static IDependencyDefinitionCollection AddScoped<TInterface, TImplementation>(this IDependencyDefinitionCollection collection, string scopeName)
            where TImplementation : TInterface
        {
            Ensure.NotNull(collection, "collection");
            collection.Add(typeof(TInterface), DependencyLifetime.NameScope(scopeName), typeof(TImplementation));
            return collection;
        }

        public static IDependencyDefinitionCollection AddScoped<TImplementation>(this IDependencyDefinitionCollection collection, string scopeName)
        {
            Ensure.NotNull(collection, "collection");
            collection.Add(typeof(TImplementation), DependencyLifetime.NameScope(scopeName), typeof(TImplementation));
            return collection;
        }

        public static IDependencyDefinitionCollection AddScoped<TImplementation>(this IDependencyDefinitionCollection collection, string scopeName, TImplementation instance)
        {
            Ensure.NotNull(collection, "collection");
            collection.Add(typeof(TImplementation), DependencyLifetime.NameScope(scopeName), instance);
            return collection;
        }

        public static IDependencyDefinitionCollection AddScopedFactory<TInterface, TFactory>(this IDependencyDefinitionCollection collection, string scopeName, TFactory factory)
            where TFactory : IFactory<TInterface>
        {
            Ensure.NotNull(collection, "collection");
            collection.Add(typeof(TInterface), DependencyLifetime.NameScope(scopeName), factory);
            return collection;
        }

        public static IDependencyDefinitionCollection AddScopedFactory<TInterface, TFactory>(this IDependencyDefinitionCollection collection, string scopeName)
            where TFactory : IFactory<TInterface>
        {
            Ensure.NotNull(collection, "collection");
            collection.Add(typeof(TInterface), DependencyLifetime.NameScope(scopeName), typeof(TFactory));
            return collection;
        }

        #endregion
    }
}
