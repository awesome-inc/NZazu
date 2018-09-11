using System;
using System.Collections.Generic;
using System.Windows.Controls;
using NZazu.Contracts.Checks;

namespace NZazu
{
    public interface INZazuWpfField: IDisposable
    {
        string Key { get; }

        Control LabelControl { get; }
        Control ValueControl { get; }
        bool IsEditable { get; }

        IEnumerable<KeyValuePair<string, string>> GetState();
        void SetStringValue(string value);
        string GetStringValue();

        ValueCheckResult Validate();
    }

    public interface INZazuWpfField<T> : INZazuWpfField
    {
        T Value { get; set; }
    }
}