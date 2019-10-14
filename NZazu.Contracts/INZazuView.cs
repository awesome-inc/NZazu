using System.Collections.Generic;
using NZazu.Contracts.Checks;

namespace NZazu.Contracts
{
    public interface INZazuView
    {
        FormDefinition FormDefinition { get; set; }
        FormData FormData { get; set; }
        bool IsReadOnly { get; set; }
        bool TrySetFocusOn(string focusOn = null, bool force = false);
        Dictionary<string, string> GetFieldValues();
        void ApplyChanges();
        ValueCheckResult Validate();
    }
}