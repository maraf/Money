using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    public struct Color
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

        public static bool operator ==(Color a, Color b)
        {
            return a.A == b.A && a.R == b.R && a.G == b.G && a.B == b.B;
        }

        public static bool operator !=(Color a, Color b)
        {
            return a.A != b.A || a.R != b.R || a.G != b.G || a.B != b.B;
        }

        public override int GetHashCode()
        {
            int hashCode = 7;
            hashCode += hashCode * 3 * A;
            hashCode += hashCode * 3 * R;
            hashCode += hashCode * 3 * G;
            hashCode += hashCode * 3 * B;
            return hashCode;
        }
    }
}
