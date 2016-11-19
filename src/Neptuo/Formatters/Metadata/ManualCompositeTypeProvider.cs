using Neptuo.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    /// <summary>
    /// The implementation of <see cref="ICompositeTypeProvider"/> which uses manual declarations for building composite types.
    /// </summary>
    public partial class ManualCompositeTypeProvider : ICompositeTypeProvider
    {
        private readonly Dictionary<Type, CompositeType> storageByType = new Dictionary<Type, CompositeType>();
        private readonly Dictionary<string, CompositeType> storageByName = new Dictionary<string, CompositeType>();

        #region ICompositeTypeProvider

        public bool TryGet(Type type, out CompositeType definition)
        {
            Ensure.NotNull(type, "type");
            return storageByType.TryGetValue(type, out definition);
        }

        public bool TryGet(string typeName, out CompositeType definition)
        {
            Ensure.NotNullOrEmpty(typeName, "typeName");
            return storageByName.TryGetValue(typeName, out definition);
        }

        #endregion

        private readonly List<ManualCompositeTypeProvider.Builder> builders = new List<Builder>();

        public ManualCompositeTypeProvider.TypeBuilder<T> Add<T>(Expression<Func<T, int>> versionGetter)
        {
            TypeBuilder<T> builder = new TypeBuilder<T>(this, versionGetter);
            builders.Add(builder);
            return builder;
        }

        public ManualCompositeTypeProvider.TypeBuilder<T> Add<T>(string typeName, Expression<Func<T, int>> versionGetter)
        {
            TypeBuilder<T> builder = new TypeBuilder<T>(this, versionGetter, typeName);
            builders.Add(builder);
            return builder;
        }

        private void AddOrReplace(Type type, string typeName, CompositeType definition)
        {
            storageByType[type] = definition;
            storageByName[typeName] = definition;
        }

        internal bool TryAddVersion(Type type, string typeName, CompositeVersion version, CompositeProperty versionProperty)
        {
            CompositeType definition;
            if (TryGet(type, out definition))
            {
                List<CompositeVersion> versions = new List<CompositeVersion>(definition.Versions);
                versions.Add(version);
                AddOrReplace(type, typeName, new CompositeType(typeName, type, versions, definition.VersionProperty));
            }
            else
            {
                AddOrReplace(type, typeName, new CompositeType(typeName, type, new List<CompositeVersion>() { version }, versionProperty));
            }

            return true;
        }
    }
}
