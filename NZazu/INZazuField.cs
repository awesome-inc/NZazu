using System.Windows.Controls;

namespace NZazu
{
    public interface INZazuField
    {
        string Key { get; }
        string Type { get; }
        string Prompt { get; }
        string Description { get; }
        Control Control { get; }
    }
}