using System;
using System.Collections.Generic;
using System.Linq;

namespace NZazu.Serializer
{
    public class NZazuTableDataSerializerBase
    {
        public void AddTableRow(Dictionary<string, string> data, Dictionary<string, string> newRow)
        {
            // clear data if ALL lines are empty
            if (data.All(x => string.IsNullOrWhiteSpace(x.Value)))
                data.Clear();

            var maxRow = data.Count == 0
                ? 0
                : data.Max(x => int.Parse(x.Key.Split(new[] {"__"}, StringSplitOptions.RemoveEmptyEntries)[1]));
            maxRow++;

            foreach (var cell in newRow)
                data.Add(cell.Key + "__" + maxRow, cell.Value);
        }
    }
}