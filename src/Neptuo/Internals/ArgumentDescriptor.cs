using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Internals
{
    internal class ArgumentDescriptor
    {
        /// <summary>
        /// The type of the parameter to the execute method.
        /// </summary>
        public Type ArgumentType { get; private set; }

        /// <summary>
        /// Whether is plain command/event handler.
        /// </summary>
        public bool IsPlain { get; private set; }

        /// <summary>
        /// Whether requires envelope wrapper.
        /// </summary>
        public bool IsEnvelope { get; private set; }

        /// <summary>
        /// Whether required context wrapper.
        /// </summary>
        public bool IsContext { get; private set; }
        
        /// <summary>
        /// Creates new instance.
        /// </summary>
        public ArgumentDescriptor(Type argumentType, bool isPlain, bool isEnvelope, bool isContext)
        {
            Ensure.NotNull(argumentType, "argumentType");
            ArgumentType = argumentType;
            IsPlain = isPlain;
            IsEnvelope = isEnvelope;
            IsContext = isContext;
        }
    }
}
