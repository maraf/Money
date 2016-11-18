using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// Describes lifetime of object in dependency container.
    /// </summary>
    public struct DependencyLifetime
    {
        /// <summary>
        /// Name of the root scope.
        /// </summary>
        public const string RootScopeName = "Root";

        /// <summary>
        /// Returns <c>true</c> if lifetime is transient.
        /// </summary>
        public readonly bool IsTransient;

        /// <summary>
        /// Returns <c>true</c> if lifetime is scoped (named or any).
        /// </summary>
        public readonly bool IsScoped;

        /// <summary>
        /// Returns <c>true</c> if lifetime is scoped and named.
        /// </summary>
        public readonly bool IsNamed;

        /// <summary>
        /// Returns name of the scope.
        /// </summary>
        public readonly string Name; 

        private DependencyLifetime(bool isScoped, string name)
        {
            IsTransient = !isScoped;
            IsScoped = isScoped;
            IsNamed = !String.IsNullOrEmpty(name);
            Name = name;
        }

        /// <summary>
        /// Transient lifetime.
        /// </summary>
        public static readonly DependencyLifetime Transient = new DependencyLifetime(false, null);

        /// <summary>
        /// Scoped lifetime.
        /// </summary>
        public static readonly DependencyLifetime Scope = new DependencyLifetime(true, null);
        
        /// <summary>
        /// Name-scoped lifetime.
        /// </summary>
        /// <param name="name">The name of the scope.</param>
        /// <returns>Name-scoped lifetime.</returns>
        public static DependencyLifetime NameScope(string name)
        {
            return new DependencyLifetime(true, name);
        }
    }
}
