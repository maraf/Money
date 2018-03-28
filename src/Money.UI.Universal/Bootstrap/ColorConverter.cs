using Neptuo.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Money
{
    public class ColorConverter : TwoWayConverter<JToken, Color>
    {
        public static Color Map(Windows.UI.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Windows.UI.Color Map(Color color)
        {
            return Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public override bool TryConvertFromOneToTwo(JToken sourceValue, out Color targetValue)
        {
            JValue value = sourceValue as JValue;
            if (value == null)
            {
                targetValue = Map(Colors.White);
                return false;
            }

            string stringValue = value.ToString();
            if (String.IsNullOrEmpty(stringValue))
            {
                targetValue = Map(Colors.White);
                return false;
            }

            byte[] byteArray = stringValue
                .Split(';')
                .Select(v => Byte.Parse(v))
                .ToArray();

            targetValue = Color.FromArgb(
                byteArray[0],
                byteArray[1],
                byteArray[2],
                byteArray[3]
            );
            return true;
        }

        public override bool TryConvertFromTwoToOne(Color sourceValue, out JToken targetValue)
        {
            targetValue = new JValue(String.Concat(
                sourceValue.A,
                ";",
                sourceValue.R,
                ";",
                sourceValue.G,
                ";",
                sourceValue.B
            ));
            return true;
        }
    }
}
