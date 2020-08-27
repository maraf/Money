using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class ColorCollection : List<Color>
    {
        public static readonly Color White = Color.FromArgb(255, 255, 255, 255);
        public static readonly Color Black = Color.FromArgb(255, 0, 0, 0);

        public ColorCollection()
        {
            Load();
        }

        private void Load()
        {
            //foreach (PropertyInfo propertyInfo in typeof(System.Drawing.Color).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty))
            //{
            //    if (propertyInfo.PropertyType == typeof(System.Drawing.Color))
            //    {
            //        var color = (System.Drawing.Color)propertyInfo.GetValue(null));
            //        Add(Color.FromArgb(color.A, color.R, color.G, color.B));
            //    }
            //}

            Add(Map(System.Drawing.Color.MediumBlue));
            Add(Map(System.Drawing.Color.MediumOrchid));
            Add(Map(System.Drawing.Color.MediumPurple));
            Add(Map(System.Drawing.Color.MediumSeaGreen));
            Add(Map(System.Drawing.Color.MediumSlateBlue));
            Add(Map(System.Drawing.Color.MediumSpringGreen));
            Add(Map(System.Drawing.Color.MediumTurquoise));
            Add(Map(System.Drawing.Color.MediumAquamarine));
            Add(Map(System.Drawing.Color.MediumVioletRed));
            Add(Map(System.Drawing.Color.MintCream));
            Add(Map(System.Drawing.Color.MistyRose));
            Add(Map(System.Drawing.Color.Moccasin));
            Add(Map(System.Drawing.Color.NavajoWhite));
            Add(Map(System.Drawing.Color.Navy));
            Add(Map(System.Drawing.Color.OldLace));
            Add(Map(System.Drawing.Color.Olive));
            Add(Map(System.Drawing.Color.MidnightBlue));
            Add(Map(System.Drawing.Color.OliveDrab));
            Add(Map(System.Drawing.Color.Maroon));
            Add(Map(System.Drawing.Color.Linen));
            Add(Map(System.Drawing.Color.LemonChiffon));
            Add(Map(System.Drawing.Color.LightBlue));
            Add(Map(System.Drawing.Color.LightCoral));
            Add(Map(System.Drawing.Color.LightCyan));
            Add(Map(System.Drawing.Color.LightGoldenrodYellow));
            Add(Map(System.Drawing.Color.LightGray));
            Add(Map(System.Drawing.Color.LightGreen));
            Add(Map(System.Drawing.Color.Magenta));
            Add(Map(System.Drawing.Color.LightPink));
            Add(Map(System.Drawing.Color.LightSeaGreen));
            Add(Map(System.Drawing.Color.LightSkyBlue));
            Add(Map(System.Drawing.Color.LightSlateGray));
            Add(Map(System.Drawing.Color.LightSteelBlue));
            Add(Map(System.Drawing.Color.LightYellow));
            Add(Map(System.Drawing.Color.Lime));
            Add(Map(System.Drawing.Color.LimeGreen));
            Add(Map(System.Drawing.Color.LightSalmon));
            Add(Map(System.Drawing.Color.Orange));
            Add(Map(System.Drawing.Color.OrangeRed));
            Add(Map(System.Drawing.Color.Orchid));
            Add(Map(System.Drawing.Color.SkyBlue));
            Add(Map(System.Drawing.Color.SlateBlue));
            Add(Map(System.Drawing.Color.SlateGray));
            Add(Map(System.Drawing.Color.Snow));
            Add(Map(System.Drawing.Color.SpringGreen));
            Add(Map(System.Drawing.Color.SteelBlue));
            Add(Map(System.Drawing.Color.Tan));
            Add(Map(System.Drawing.Color.Silver));
            Add(Map(System.Drawing.Color.Teal));
            Add(Map(System.Drawing.Color.Tomato));
            Add(Map(System.Drawing.Color.Transparent));
            Add(Map(System.Drawing.Color.Turquoise));
            Add(Map(System.Drawing.Color.Violet));
            Add(Map(System.Drawing.Color.Wheat));
            Add(Map(System.Drawing.Color.White));
            Add(Map(System.Drawing.Color.WhiteSmoke));
            Add(Map(System.Drawing.Color.Thistle));
            Add(Map(System.Drawing.Color.Sienna));
            Add(Map(System.Drawing.Color.SeaShell));
            Add(Map(System.Drawing.Color.SeaGreen));
            Add(Map(System.Drawing.Color.PaleGoldenrod));
            Add(Map(System.Drawing.Color.PaleGreen));
            Add(Map(System.Drawing.Color.PaleTurquoise));
            Add(Map(System.Drawing.Color.PaleVioletRed));
            Add(Map(System.Drawing.Color.PapayaWhip));
            Add(Map(System.Drawing.Color.PeachPuff));
            Add(Map(System.Drawing.Color.Peru));
            Add(Map(System.Drawing.Color.Pink));
            Add(Map(System.Drawing.Color.Plum));
            Add(Map(System.Drawing.Color.PowderBlue));
            Add(Map(System.Drawing.Color.Purple));
            Add(Map(System.Drawing.Color.Red));
            Add(Map(System.Drawing.Color.RosyBrown));
            Add(Map(System.Drawing.Color.RoyalBlue));
            Add(Map(System.Drawing.Color.SaddleBrown));
            Add(Map(System.Drawing.Color.Salmon));
            Add(Map(System.Drawing.Color.SandyBrown));
            Add(Map(System.Drawing.Color.Yellow));
            Add(Map(System.Drawing.Color.LavenderBlush));
            Add(Map(System.Drawing.Color.LawnGreen));
            Add(Map(System.Drawing.Color.Khaki));
            Add(Map(System.Drawing.Color.DarkMagenta));
            Add(Map(System.Drawing.Color.DarkKhaki));
            Add(Map(System.Drawing.Color.DarkGreen));
            Add(Map(System.Drawing.Color.DarkGray));
            Add(Map(System.Drawing.Color.DarkGoldenrod));
            Add(Map(System.Drawing.Color.Lavender));
            Add(Map(System.Drawing.Color.DarkBlue));
            Add(Map(System.Drawing.Color.Cyan));
            Add(Map(System.Drawing.Color.Crimson));
            Add(Map(System.Drawing.Color.Cornsilk));
            Add(Map(System.Drawing.Color.CornflowerBlue));
            Add(Map(System.Drawing.Color.Coral));
            Add(Map(System.Drawing.Color.Chocolate));
            Add(Map(System.Drawing.Color.DarkOliveGreen));
            Add(Map(System.Drawing.Color.Chartreuse));
            Add(Map(System.Drawing.Color.BurlyWood));
            Add(Map(System.Drawing.Color.Brown));
            Add(Map(System.Drawing.Color.BlueViolet));
            Add(Map(System.Drawing.Color.Blue));
            Add(Map(System.Drawing.Color.BlanchedAlmond));
            Add(Map(System.Drawing.Color.Black));
            Add(Map(System.Drawing.Color.Bisque));
            Add(Map(System.Drawing.Color.Beige));
            Add(Map(System.Drawing.Color.Azure));
            Add(Map(System.Drawing.Color.Aquamarine));
            Add(Map(System.Drawing.Color.Aqua));
            Add(Map(System.Drawing.Color.AntiqueWhite));
            Add(Map(System.Drawing.Color.AliceBlue));
            Add(Map(System.Drawing.Color.CadetBlue));
            Add(Map(System.Drawing.Color.DarkOrange));
            Add(Map(System.Drawing.Color.DarkCyan));
            Add(Map(System.Drawing.Color.DarkRed));
            Add(Map(System.Drawing.Color.Ivory));
            Add(Map(System.Drawing.Color.Indigo));
            Add(Map(System.Drawing.Color.IndianRed));
            Add(Map(System.Drawing.Color.HotPink));
            Add(Map(System.Drawing.Color.Honeydew));
            Add(Map(System.Drawing.Color.DarkOrchid));
            Add(Map(System.Drawing.Color.Green));
            Add(Map(System.Drawing.Color.Gray));
            Add(Map(System.Drawing.Color.Goldenrod));
            Add(Map(System.Drawing.Color.Gold));
            Add(Map(System.Drawing.Color.GhostWhite));
            Add(Map(System.Drawing.Color.Gainsboro));
            Add(Map(System.Drawing.Color.Fuchsia));
            Add(Map(System.Drawing.Color.GreenYellow));
            Add(Map(System.Drawing.Color.FloralWhite));
            Add(Map(System.Drawing.Color.ForestGreen));
            Add(Map(System.Drawing.Color.DarkSalmon));
            Add(Map(System.Drawing.Color.DarkSeaGreen));
            Add(Map(System.Drawing.Color.DarkSlateBlue));
            Add(Map(System.Drawing.Color.DarkSlateGray));
            Add(Map(System.Drawing.Color.DarkTurquoise));
            Add(Map(System.Drawing.Color.DarkViolet));
            Add(Map(System.Drawing.Color.YellowGreen));
            Add(Map(System.Drawing.Color.DeepSkyBlue));
            Add(Map(System.Drawing.Color.DimGray));
            Add(Map(System.Drawing.Color.DeepPink));
            Add(Map(System.Drawing.Color.DodgerBlue));
            Add(Map(System.Drawing.Color.Firebrick));
        }

        private Color Map(System.Drawing.Color color) => Color.FromArgb(color.A, color.R, color.G, color.B);
    }
}
