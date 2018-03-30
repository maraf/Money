using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Converters
{
    /// <summary>
    /// The enumeration of supported enum serialization and deserialization types.
    /// </summary>
    public enum JsonEnumConverterType
    {
        /// <summary>
        /// The enum value is processed as a name.
        /// </summary>
        UseTextName,

        /// <summary>
        /// The enum value is processed as an undelaying type value.
        /// </summary>
        UseInderlayingValue
    }
}
