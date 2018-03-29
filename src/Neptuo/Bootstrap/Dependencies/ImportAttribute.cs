using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Dependencies
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ImportAttribute : Attribute
    {
        public string Name { get; private set; }

        public ImportAttribute()
        { }

        public ImportAttribute(string name)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Name = name;
        }
    }
}
