using Neptuo.Bootstrap.Dependencies.Providers.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Dependencies.Providers
{
    public class PropertyDescriptorProvider : IDependencyDescriptorProvider
    {
        public IEnumerable<IDependencyImportDescriptor> GetImports(Type taskType)
        {
            List<IDependencyImportDescriptor> result = new List<IDependencyImportDescriptor>();
            foreach (PropertyInfo propertyInfo in taskType.GetProperties())
            {
                ImportAttribute attribute = propertyInfo.GetCustomAttribute<ImportAttribute>();
                if (attribute != null)
                    result.Add(new PropertyImportDescriptor(new ImportAttributeTarget(propertyInfo, attribute), propertyInfo));
            }

            return result;
        }

        public IEnumerable<IDependencyExportDescriptor> GetExports(Type taskType)
        {
            List<IDependencyExportDescriptor> result = new List<IDependencyExportDescriptor>();
            foreach (PropertyInfo propertyInfo in taskType.GetProperties())
            {
                ExportAttribute attribute = propertyInfo.GetCustomAttribute<ExportAttribute>();
                if (attribute != null)
                    result.Add(new PropertyExportDescriptor(new ExportAttributeTarget(propertyInfo, attribute), propertyInfo));
            }

            return result;
        }
    }
}
