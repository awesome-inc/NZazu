using NZazu.Contracts;

namespace NZazu
{
    public interface INZazuFieldFactory
    {
        INZazuField CreateField(FieldDefinition fieldDefinition);
    }
}