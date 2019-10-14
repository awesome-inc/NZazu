using System.Collections.Generic;

namespace NZazu.Contracts
{
    public class BehaviorDefinition
    {
        public string Name { get; set; }
        public Dictionary<string, string> Settings { get; set; }
    }
}