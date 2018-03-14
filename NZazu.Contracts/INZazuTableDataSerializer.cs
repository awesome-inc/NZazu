using System.Collections.Generic;

namespace NZazu.Contracts
{
    public interface INZazuTableDataSerializer
    {
        string Serialize(Dictionary<string, string> data);
        Dictionary<string, string> Deserialize(string value);

        void AddTableRow(Dictionary<string, string> data, Dictionary<string, string> newRow);
    }
}