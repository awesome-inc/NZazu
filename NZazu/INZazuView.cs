using NZazu.Contracts;

namespace NZazu
{
    public interface INZazuView
    {
        FormDefinition FormDefinition { get; set; }
        INZazuField GetField(string fieldKey);
    }
}