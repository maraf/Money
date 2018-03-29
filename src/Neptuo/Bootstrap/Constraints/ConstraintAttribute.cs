using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Bootstrap.Constraints
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ConstraintAttribute : Attribute
    {
        public Type Type { get; private set; }

        public ConstraintAttribute()
        { }

        public ConstraintAttribute(Type type)
        {
            Type = type;
        }

        public virtual Type GetConstraintType()
        {
            if(Type != null)
                return Type;

            throw new ArgumentNullException("Type");
        }
    }
}
