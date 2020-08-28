using Money;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Converters
{
    public class ColorConverter : TwoWayConverter<JToken, Color>
    {
        public override bool TryConvertFromOneToTwo(JToken sourceValue, out Color targetValue)
        {
            JValue value = sourceValue as JValue;
            if (value == null)
            {
                targetValue = Color.FromArgb(255, 255, 255, 255);
                return false;
            }

            string stringValue = value.ToString();
            if (String.IsNullOrEmpty(stringValue))
            {
                targetValue = Color.FromArgb(255, 255, 255, 255);
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
