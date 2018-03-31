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
            byte[] data = new byte[] { color.R, color.G, color.B, color.A };
            return "#" + BitConverter.ToString(data).Replace("-", string.Empty);
        }
    }
}
