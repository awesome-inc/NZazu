using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazu
{
    public interface INZazuView
    {
        FormDefinition FormDefinition { get; set; }
        IDictionary<string, string> FormData { get; set; }
            
        INZazuFieldFactory FieldFactory { get; set; }
        INZazuLayoutStrategy LayoutStrategy { get; set; }

        INZazuField GetField(string fieldKey);
        void ApplyChanges();
        void Validate();
    }
}