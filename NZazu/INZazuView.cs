using NZazu.Contracts;
using NZazu.Contracts.Checks;

namespace NZazu
{
    public interface INZazuView
    {
        FormDefinition FormDefinition { get; set; }
        FormData FormData { get; set; }
            
        INZazuFieldFactory FieldFactory { get; set; }
        INZazuLayoutStrategy LayoutStrategy { get; set; }

        INZazuField GetField(string fieldKey);
        void ApplyChanges();

        /// <exception cref="ValidationException"></exception>
        void Validate();
    }
}