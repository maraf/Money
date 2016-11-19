using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    partial class ReflectionCompositeTypeProvider
    {
        private class PropertyDescriptor
        {
            public CompositePropertyAttribute Attribute { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
        }
    }
}
