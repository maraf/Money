using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Dependencies.Providers.Targets
{
    public class ImportAttributeTarget : IDependencyTarget
    {
        public Type Type { get; private set; }
        public string Name { get; private set; }

        public ImportAttributeTarget(PropertyInfo propertyInfo, ImportAttribute attribute)
        {
            Ensure.NotNull(propertyInfo, "propertyInfo");
            Ensure.NotNull(attribute, "attribute");
            Type = propertyInfo.PropertyType;
            Name = attribute.Name;
        }

        public bool Equals(IDependencyTarget other)
        {
            ImportAttributeTarget target = other as ImportAttributeTarget;
            if (target == null)
                return false;

            return Type == target.Type && Name == target.Name;
        }
    }
}
