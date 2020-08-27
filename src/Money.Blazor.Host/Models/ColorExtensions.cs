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

        public static Color ToAccentColor(this Color color)
            => SelectAccent(color, ColorCollection.White, ColorCollection.Black);

        public static T SelectAccent<T>(this Color color, T light, T dark)
        {
            if ((color.R + color.G + color.B) / 3 < 90)
                return light;

            return dark;
        }
    }
}
