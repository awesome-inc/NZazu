using System;
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

        // option
        public string[] Values { get; set; }

        public CheckDefinition[] Checks { get; set; }
        [Obsolete("Please choose '" + nameof(Behaviors) + "' instead")]
        public BehaviorDefinition Behavior { get; set; }
        public List<BehaviorDefinition> Behaviors { get; set; }

        public Dictionary<string,string> Settings { get; set; }

        // group fields
        public FieldDefinition[] Fields { get; set; }
        public string Layout { get; set; }
    }
}