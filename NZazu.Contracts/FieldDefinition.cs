using System;
using System.Collections.Generic;
using System.Linq;

namespace NZazu.Contracts
{
    public class FieldDefinition
    {
        public string Key { get; set; }
        public string Type { get; set; }

        public string Prompt { get; set; }
        public string Hint { get; set; }
        public string Description { get; set; }

        // option
        public IEnumerable<string> Values { get; set; } = Enumerable.Empty<string>();

        public IEnumerable<CheckDefinition> Checks { get; set; } = Enumerable.Empty<CheckDefinition>();
        public IEnumerable<BehaviorDefinition> Behaviors { get; set; } = Enumerable.Empty<BehaviorDefinition>();

        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();

        // group fields
        public IEnumerable<FieldDefinition> Fields { get; set; } = Enumerable.Empty<FieldDefinition>();
        public string Layout { get; set; }
    }
}