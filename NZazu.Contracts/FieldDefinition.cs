using System.Collections.Generic;
using System.Linq;

namespace NZazu.Contracts
{
    public class FieldDefinition
    {
        // required values for a new field
        public string Key { get; set; }
        public string Type { get; set; }

        // additional values for tooltip and hints
        public string Prompt { get; set; }
        public string Hint { get; set; }
        public string Description { get; set; }

        // options & settings
        public IEnumerable<string> Values { get; set; } = Enumerable.Empty<string>();
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();

        // behaviors and checks
        public IEnumerable<CheckDefinition> Checks { get; set; } = Enumerable.Empty<CheckDefinition>();
        public IEnumerable<BehaviorDefinition> Behaviors { get; set; } = Enumerable.Empty<BehaviorDefinition>();

        // group fields and layout definition
        public IEnumerable<FieldDefinition> Fields { get; set; } = Enumerable.Empty<FieldDefinition>();
        public string Layout { get; set; }
    }
}