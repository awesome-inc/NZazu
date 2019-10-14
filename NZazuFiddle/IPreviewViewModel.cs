using NZazu;

namespace NZazuFiddle
{
    public interface IPreviewViewModel
        : IHaveFormDefinition, IHaveFormData
    {
        INZazuWpfFieldFactory FieldFactory { get; }
        bool IsReadOnly { get; set; }
    }
}