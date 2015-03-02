using System.Collections.Generic;
using NZazu.Contracts;
using NZazu.Contracts.Checks;

namespace NZazu
{
    public interface INZazuWpfView
    {
        FormDefinition FormDefinition { get; set; }
        FormData FormData { get; set; }
        bool IsReadOnly { get; set; }
            
        INZazuWpfFieldFactory FieldFactory { get; set; }
        IResolveLayout ResolveLayout { get; set; }

        INZazuWpfField GetField(string key);
        bool TryGetField(string key, out INZazuWpfField field);
        bool TrySetFocusOn(string key);

        Dictionary<string, string> GetFieldValues();
        void ApplyChanges();

        ValueCheckResult Validate();
    }
}