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
        Control LabelControl { get; }
        Control ValueControl { get; }
    }
}