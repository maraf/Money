using Neptuo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// Throw when user tries to set not suppoted property
    /// </summary>
    [Serializable]
    public class UserPropertyNotSupportedException : AggregateRootException
    {
        /// <summary>
        /// Gets a key of unsupported property.
        /// </summary>
        public string PropertyKey { get; }

        /// <summary>
        /// Creates new empty instance.
        /// </summary>
        public UserPropertyNotSupportedException(string propertyKey)
        {
            PropertyKey = propertyKey;
        }
    }
}
