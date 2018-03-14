using System.Collections.Generic;
using Newtonsoft.Json;
using NZazu.Contracts;
using NZazu.Serializer;

namespace NZazu.JsonSerializer
{
    public class NZazuTableDataJsonSerializer
        : NZazuTableDataSerializerBase
        , INZazuTableDataSerializer
    {
        public string Serialize(Dictionary<string, string> data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public Dictionary<string, string> Deserialize(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return new Dictionary<string, string>();

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
        }
    }
}
