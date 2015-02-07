using System.Collections.Generic;
using System.Windows.Controls;

namespace NZazu
{
    public interface INZazuField
    {
        string Key { get; }
        string Type { get; }
        string Prompt { get; }
        string Hint { get; }
        string Description { get; }

        string StringValue { get; set; }
        void Validate();

        Control LabelControl { get; }
        Control ValueControl { get; }
        Dictionary<string, string> Settings { get; }
    }

    public interface INZazuField<T> : INZazuField
    {
        T Value { get; set; }
    }
}