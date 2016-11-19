using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    /// <summary>
    /// Marks model version property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CompositeVersionAttribute : Attribute
    { }
}
