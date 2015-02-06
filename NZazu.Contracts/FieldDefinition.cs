using System.Collections.Generic;
using NZazu.Contracts.Checks;

namespace NZazu.Contracts
{
    public class FieldDefinition
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string Prompt { get; set; }
        public string Hint { get; set; }
        public string Description { get; set; }

        public IEnumerable<IValueCheck> Checks { get; set; }

        public string Format { get; set; }
    }
}