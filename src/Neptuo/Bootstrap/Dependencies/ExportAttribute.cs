using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Dependencies
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExportAttribute : Attribute
    {
        public string Name { get; private set; }

        public ExportAttribute()
        { }

        public ExportAttribute(string name)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Name = name;
        }
    }
}
