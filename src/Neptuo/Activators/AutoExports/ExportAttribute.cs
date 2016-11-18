using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators.AutoExports
{
    /// <summary>
    /// Auto dependency container wiring.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExportAttribute : Attribute
    {
        /// <summary>
        /// Type to export service as.
        /// </summary>
        public Type RequiredType { get; private set; }

        /// <summary>
        /// Creates new instance that exports service as <paramref name="requiredType"/>.
        /// </summary>
        /// <param name="requiredType">Type to export service as.</param>
        public ExportAttribute(Type requiredType)
        {
            Ensure.NotNull(requiredType, "requiredType");
            RequiredType = requiredType;
        }
    }
}
