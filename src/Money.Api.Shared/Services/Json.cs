using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Money.Services
{
    public class Json
    {
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public T Deserialize<T>(string json)
            => JsonSerializer.Deserialize<T>(json);

        public string Serialize(object instance)
            => JsonSerializer.Serialize(instance);
    }
}
