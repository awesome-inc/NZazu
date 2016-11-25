using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts.Checks;

namespace NZazu
{
    public interface INZazuWpfField
    {
        string Key { get; }
        string Type { get; }
        string Prompt { get; }
        string Hint { get; }
        string Description { get; }

        bool IsEditable { get; }
        string StringValue { get; set; }
        ValueCheckResult Validate();

        Control LabelControl { get; }
        Control ValueControl { get; }
        Dictionary<string, string> Settings { get; }
        INZazuWpfFieldBehavior Behavior { get; set; }
        DependencyProperty ContentProperty { get; }

        void DisposeField();
    }

    public interface INZazuWpfField<T> : INZazuWpfField
    {
        T Value { get; set; }
    }
}