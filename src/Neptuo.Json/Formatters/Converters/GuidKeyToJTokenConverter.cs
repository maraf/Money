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
    /// The converter from the <see cref="GuidKey"/> to the <see cref="JToken"/>.
    /// </summary>
    public class GuidKeyToJTokenConverter : KeyToJObjectConverter<GuidKey>, IConverter<GuidKey, JToken>, IConverter<JToken, GuidKey>
    {
        protected override bool TryConvert(GuidKey source, out JObject target)
        {
            target = new JObject();
            target[JsonName.Type] = source.Type;
            if (source.IsEmpty)
                target[JsonName.GuidValue] = null;
            else
                target[JsonName.GuidValue] = source.Guid.ToString();

            return true;
        }

        protected override bool TryConvert(JObject source, out GuidKey target)
        {
            string type = source.Value<string>(JsonName.Type);
            string value = source.Value<string>(JsonName.GuidValue);

            Guid guid;
            if (Guid.TryParse(value, out guid))
                target = GuidKey.Create(guid, type);
            else
                target = GuidKey.Empty(type);

            return true;
        }
    }
}
