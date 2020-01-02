using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    /// <summary>
    /// The implementation of <see cref="ICompositeTypeProvider"/> which uses reflection and attributes from <see cref="Neptuo.Formatters.Metadata"/>
    /// to compose composite types on the fly, without the to register types manually.
    /// </summary>
    public partial class ReflectionCompositeTypeProvider : ICompositeTypeProvider
    {
        private readonly object storageLock = new object();
        private readonly Dictionary<Type, CompositeType> storageByType = new Dictionary<Type, CompositeType>();
        private readonly Dictionary<string, CompositeType> storageByName = new Dictionary<string, CompositeType>();
        private readonly ICompositeDelegateFactory delegateFactory;
        private readonly BindingFlags? bindingFlags;
        private readonly ILogFactory logFactory;

        /// <summary>
        /// Creates new instance with <paramref name="delegateFactory"/> for property and constructor delegates.
        /// </summary>
        /// <param name="delegateFactory">The factory for delegates for fast access.</param>
        public ReflectionCompositeTypeProvider(ICompositeDelegateFactory delegateFactory)
            : this(delegateFactory, new DefaultLogFactory())
        { }

        /// <summary>
        /// Creates new instance with <paramref name="delegateFactory"/> for property and constructor delegates.
        /// </summary>
        /// <param name="delegateFactory">The factory for delegates for fast access.</param>
        /// <param name="logFactory">The factory for the log to write debug information.</param>
        public ReflectionCompositeTypeProvider(ICompositeDelegateFactory delegateFactory, ILogFactory logFactory)
        {
            Ensure.NotNull(delegateFactory, "delegateFactory");
            Ensure.NotNull(logFactory, "logFactory");
            this.delegateFactory = delegateFactory;
            this.logFactory = logFactory.Scope("ReflectionCompositeTypeProvider").Factory;
        }

        /// <summary>
        /// Creates new instance with <paramref name="delegateFactory"/> for property and constructor delegates.
        /// </summary>
        /// <param name="delegateFactory">The factory for delegates for fast access.</param>
        /// <param name="bindingFlags">The binding flags for accessing reflection.</param>
        public ReflectionCompositeTypeProvider(ICompositeDelegateFactory delegateFactory, BindingFlags bindingFlags)
            : this(delegateFactory, bindingFlags, new DefaultLogFactory())
        { }

        /// <summary>
        /// Creates new instance with <paramref name="delegateFactory"/> for property and constructor delegates.
        /// </summary>
        /// <param name="delegateFactory">The factory for delegates for fast access.</param>
        /// <param name="bindingFlags">The binding flags for accessing reflection.</param>
        /// <param name="logFactory">The factory for the log to write debug information.</param>
        public ReflectionCompositeTypeProvider(ICompositeDelegateFactory delegateFactory, BindingFlags bindingFlags, ILogFactory logFactory)
        {
            Ensure.NotNull(delegateFactory, "delegateFactory");
            Ensure.NotNull(logFactory, "logFactory");
            this.delegateFactory = delegateFactory;
            this.bindingFlags = bindingFlags;
            this.logFactory = logFactory.Scope("ReflectionCompositeTypeProvider").Factory;
        }

        #region ICompositeTypeProvider

        public bool TryGet(Type type, out CompositeType definition)
        {
            Ensure.NotNull(type, "type");
            if (storageByType.TryGetValue(type, out definition))
                return true;

            definition = BuildType(type);
            if (definition == null)
                return false;

            lock (storageLock)
            {
                CompositeType existingDefinition;
                if (storageByType.TryGetValue(type, out existingDefinition))
                {
                    definition = existingDefinition;
                    return true;
                }

                storageByType[type] = definition;
                storageByName[definition.Name] = definition;
            }

            return true;
        }

        public bool TryGet(string typeName, out CompositeType definition)
        {
            Ensure.NotNullOrEmpty(typeName, "typeName");
            return storageByName.TryGetValue(typeName, out definition);
        }

        #endregion

        private CompositeType BuildType(Type type)
        {
            ILog log = logFactory.Scope("BuildType");

            string typeName = type.FullName;
            log.Info("Building type '{0}'.", typeName);

            CompositeTypeAttribute typeAttribute = type.GetTypeInfo().GetCustomAttribute<CompositeTypeAttribute>();
            if (typeAttribute != null)
                typeName = typeAttribute.Name;

            Dictionary<int, ConstructorInfo> constructors = GetConstructors(type);
            log.Info("Constructors '{0}'.", constructors.Count);

            IEnumerable<PropertyDescriptor> properties = GetProperties(type);
            log.Info("Properties '{0}'.", properties.Count());

            List<CompositeVersion> versions = new List<CompositeVersion>();
            foreach (KeyValuePair<int, ConstructorInfo> constructor in constructors)
            {
                IEnumerable<PropertyDescriptor> versionProperties;

                // Create version from annotated properties.
                if (TryFindAnnotatedProperties(properties, constructor.Value.GetParameters().Length, constructor.Key, out versionProperties))
                {
                    log.Info("Version '{0}' from annotated properties.", constructor.Key);
                    versions.Add(BuildVersion(constructor.Key, constructor.Value, versionProperties));
                    continue;
                }

                // Create version from property name match.
                if(TryFindNamedProperties(properties, constructor.Value.GetParameters(), out versionProperties))
                {
                    log.Info("Version '{0}' from conventionally properties.", constructor.Key);
                    versions.Add(BuildVersion(constructor.Key, constructor.Value, versionProperties));
                    continue;
                }

                throw new MismatchVersionConstructorException(type, constructor.Key);
            }

            CompositeProperty versionProperty = null;
            PropertyDescriptor versionPropertyDescriptor = properties.FirstOrDefault(p => p.PropertyInfo.GetCustomAttribute<CompositeVersionAttribute>() != null);
            if (versionPropertyDescriptor == null)
            {
                if (versions.Count == 1)
                {
                    log.Info("Implicit version property.");
                    versionProperty = new CompositeProperty(0, "_Version", typeof(int), model => 1);
                }
                else
                {
                    log.Warning("Found '{0}' versions on the '{1}'.", versions.Count, typeName);
                    throw new MissingVersionPropertyException(type);
                }
            }
            else
            {
                Func<object, object> getter = delegateFactory.CreatePropertyGetter(versionPropertyDescriptor.PropertyInfo);

                // Use setter for version only when setter method is present and is public.
                Action<object, object> setter = null;
                if (versionPropertyDescriptor.PropertyInfo.CanWrite && versionPropertyDescriptor.PropertyInfo.SetMethod != null && versionPropertyDescriptor.PropertyInfo.SetMethod.IsPublic)
                    setter = delegateFactory.CreatePropertySetter(versionPropertyDescriptor.PropertyInfo);

                if (setter == null)
                    versionProperty = new CompositeProperty(0, versionPropertyDescriptor.PropertyInfo.Name, versionPropertyDescriptor.PropertyInfo.PropertyType, getter);
                else
                    versionProperty = new CompositeProperty(0, versionPropertyDescriptor.PropertyInfo.Name, versionPropertyDescriptor.PropertyInfo.PropertyType, getter, setter);
            }

            versions.Sort((v1, v2) => v1.Version.CompareTo(v2.Version));
            return new CompositeType(typeName, type, versions, versionProperty);
        }

        private bool TryFindAnnotatedProperties(IEnumerable<PropertyDescriptor> allProperties, int count, int version, out IEnumerable<PropertyDescriptor> versionProperties)
        {
            versionProperties = allProperties.Where(p => p.Attribute != null && p.Attribute.Version == version);
            return versionProperties.Count() == count;
        }

        private bool TryFindNamedProperties(IEnumerable<PropertyDescriptor> allProperties, ParameterInfo[] parameterInfos, out IEnumerable<PropertyDescriptor> versionProperties)
        {
            IEnumerable<string> propertyNames = parameterInfos.Select(p => p.Name.ToLowerInvariant());
            List<PropertyDescriptor> result = new List<PropertyDescriptor>();
            int index = 0;
            foreach (string propertyName in propertyNames)
            {
                PropertyDescriptor property = allProperties.FirstOrDefault(p => p.PropertyInfo.Name.ToLowerInvariant() == propertyName);
                if(property != null)
                {
                    result.Add(property);
                }

                index++;
            }

            if (result.Count == propertyNames.Count())
            {
                versionProperties = result;
                return true;
            }

            versionProperties = null;
            return false;
        }

        private CompositeVersion BuildVersion(int version, ConstructorInfo constructorInfo, IEnumerable<PropertyDescriptor> properties)
        {
            return new CompositeVersion(
                version,
                new CompositeConstructor(delegateFactory.CreateConstructorFactory(constructorInfo)),
                properties
                    .Select(p => new CompositeProperty(p.Attribute.Index, p.PropertyInfo.Name, p.PropertyInfo.PropertyType, delegateFactory.CreatePropertyGetter(p.PropertyInfo)))
                    .ToList()
            );
        }

        private Dictionary<int, ConstructorInfo> GetConstructors(Type type)
        {
            IEnumerable<ConstructorInfo> constructorInfos = bindingFlags == null 
                ? type.GetConstructors() 
                : type.GetConstructors(bindingFlags.Value);

            ConstructorInfo defaultConstructor = null;

            Dictionary<int, ConstructorInfo> constructors = new Dictionary<int, ConstructorInfo>();
            foreach (ConstructorInfo constructorInfo in constructorInfos)
            {
                CompositeConstructorAttribute constructorAttribute = constructorInfo.GetCustomAttribute<CompositeConstructorAttribute>();
                if (constructorAttribute == null)
                {
                    defaultConstructor = constructorInfo;
                }
                else
                {
                    constructors[constructorAttribute.Version] = constructorInfo;
                    defaultConstructor = null;
                }
            }

            if (defaultConstructor != null && !constructors.ContainsKey(1))
                constructors[1] = defaultConstructor;

            return constructors;
        }

        private IEnumerable<PropertyDescriptor> GetProperties(Type type)
        {
            List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
            IEnumerable<PropertyInfo> propertyInfos = bindingFlags == null 
                ? type.GetProperties()
                : type.GetProperties(bindingFlags.Value);

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                bool any = false;
                IEnumerable<CompositePropertyAttribute> attributes = propertyInfo.GetCustomAttributes<CompositePropertyAttribute>();
                foreach (CompositePropertyAttribute attribute in attributes)
                {
                    any = true;
                    properties.Add(new PropertyDescriptor()
                    {
                        PropertyInfo = propertyInfo,
                        Attribute = attribute
                    });
                }

                if (!any)
                {
                    properties.Add(new PropertyDescriptor()
                    {
                        PropertyInfo = propertyInfo,
                        Attribute = new CompositePropertyAttribute(1) 
                        { 
                            Version = 1
                        }
                    });
                }
            }

            return properties;
        }
    }
}
