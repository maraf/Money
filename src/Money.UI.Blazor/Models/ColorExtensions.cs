using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public static class ColorExtensions
    {
        public static string ToHashCode(this Color color)
        {
            byte[] data = color.A == Byte.MaxValue
                ? new byte[] { color.R, color.G, color.B }
                : new byte[] { color.R, color.G, color.B, color.A };

            return "#" + BitConverter.ToString(data).Replace("-", string.Empty);
        }
    }
}
