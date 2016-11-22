using System.Collections.Generic;

namespace NZazu
{
    public interface INZazuDataSerializer
    {
        string Serialize(Dictionary<string, object> data);
        Dictionary<string, string> Deserialize(string value);
    }
}