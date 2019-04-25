using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    public struct Color : IEquatable<Color>
    {
        public byte A { get; set; }
        public byte B { get; set; }
        public byte G { get; set; }
        public byte R { get; set; }

        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            return new Color()
            {
                A = a,
                R = r,
                G = g,
                B = b
            };
        }

        public override bool Equals(object obj) => obj is Color && Equals((Color)obj);

        public bool Equals(Color other) => A == other.A && B == other.B && G == other.G && R == other.R;

        public override int GetHashCode()
        {
            var hashCode = 146518540;
            hashCode = hashCode * -1521134295 + A.GetHashCode();
            hashCode = hashCode * -1521134295 + B.GetHashCode();
            hashCode = hashCode * -1521134295 + G.GetHashCode();
            hashCode = hashCode * -1521134295 + R.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Color color1, Color color2) => color1.Equals(color2);

        public static bool operator !=(Color color1, Color color2) => !(color1 == color2);
    }
}
