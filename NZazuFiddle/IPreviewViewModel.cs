using NZazu;

namespace NZazuFiddle
{
    public interface IPreviewViewModel 
        : IHaveFormDefinition, IHaveFormData
    {
        INZazuFieldFactory FieldFactory { get; }
        INZazuLayoutStrategy LayoutStrategy { get; }
    }
}