using System.Collections.Generic;
using NZazu.Contracts;

namespace Sample
{
    public interface INZazuSample
    {
        string Name { get; }
        string Description { get; }
        FormDefinition FormDefinition { get; }

        IDictionary<string, string> FormData { get; set; }
        void ApplyChanges();
    }
}