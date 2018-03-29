using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Dependencies.Providers
{
    public interface IDependencyDescriptorProvider
    {
        IEnumerable<IDependencyImportDescriptor> GetImports(Type taskType);
        IEnumerable<IDependencyExportDescriptor> GetExports(Type taskType);
    }
}
