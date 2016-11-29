using System.Collections.Generic;
using Newtonsoft.Json;

namespace NZazu.JsonSerializer
{
    public class NZazuJsonSerializer
        : INZazuDataSerializer
    {
        public string Serialize(Dictionary<string, string> data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public Dictionary<string, string> Deserialize(string value)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
        }
    }
}
