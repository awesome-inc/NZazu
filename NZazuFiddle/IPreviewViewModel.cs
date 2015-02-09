using NZazu;

namespace NZazuFiddle
{
    public interface IPreviewViewModel 
        : IHaveFormDefinition, IHaveFormData
    {
        INZazuWpfFieldFactory FieldFactory { get; }
        INZazuWpfLayoutStrategy LayoutStrategy { get; }
    }
}