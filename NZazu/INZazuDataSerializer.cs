using System.Collections.Generic;

namespace NZazu
{
    public interface INZazuDataSerializer
    {
        string Serialize(Dictionary<string, string> data);
        Dictionary<string, string> Deserialize(string value);
    }
}