using System.Collections.Generic;

namespace NZazu.Contracts
{
    public class FieldDefinition
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string Prompt { get; set; }
        public string Hint { get; set; }
        public string Description { get; set; }

        public CheckDefinition[] Checks { get; set; }
        public BehaviorDefinition Behavior { get; set; }

        public Dictionary<string,string> Settings { get; set; }

        public FieldDefinition[] Fields { get; set; }
    }
}