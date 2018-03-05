using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazuFiddle.TemplateManagement.Data
{
    internal class ElasticSearchFormDocument
    {
        public string Id { get; set; }
        public FormDefinition FormDefinition { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }
}
