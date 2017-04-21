using Neptuo.Converters;
using Neptuo.Models.Keys;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Converters
{
    /// <summary>
    /// The converter from the <see cref="StringKey"/> to the <see cref="JToken"/>.
    /// </summary>
    public class StringKeyToJTokenConverter : KeyToJObjectConverter<StringKey>, IConverter<StringKey, JToken>, IConverter<JToken, StringKey>
    {
        protected override bool TryConvert(StringKey source, out JObject target)
        {
            target = new JObject();
            target[JsonName.Type] = source.Type;
            if (source.IsEmpty)
                target[JsonName.StringValue] = null;
            else
                target[JsonName.StringValue] = source.Identifier;

            return true;
        }

        protected override bool TryConvert(JObject source, out StringKey target)
        {
            string type = source.Value<string>(JsonName.Type);
            string value = source.Value<string>(JsonName.StringValue);

            if (value == null)
                target = StringKey.Empty(type);
            else
                target = StringKey.Create(value, type);

            return true;
        }
    }
}
