using NZazu.Contracts;

namespace NZazu
{
    public interface INZazuWpfFieldFactory
    {
        INZazuWpfField CreateField(FieldDefinition fieldDefinition);
        INZazuDataSerializer Serializer { get; set; }
    }
}