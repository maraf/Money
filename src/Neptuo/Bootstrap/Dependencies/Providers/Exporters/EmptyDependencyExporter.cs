using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Dependencies.Providers.Exporters
{
    public class EmptyDependencyExporter : IDependencyExporter
    {
        public void Export(IDependencyExportDescriptor exportDescriptor, object value)
        { }
    }
}
