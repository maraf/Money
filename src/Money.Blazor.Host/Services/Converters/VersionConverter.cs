using Neptuo.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services.Converters
{
    public class VersionConverter : DefaultConverter<Version, string>
    {
        public override bool TryConvert(Version sourceValue, out string targetValue)
        {
            targetValue = null;
            if (sourceValue != null)
            {
                string text = sourceValue.Revision == 0
                    ? sourceValue.ToString(3)
                    : sourceValue.ToString(4);

                targetValue = $"v{text}";
            }

            return true;
        }
    }
}
