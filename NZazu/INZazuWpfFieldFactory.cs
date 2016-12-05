using NZazu.Contracts;

namespace NZazu
{
    public interface INZazuWpfFieldFactory
    {
        INZazuWpfField CreateField(FieldDefinition fieldDefinition);

        INZazuWpfFieldBehaviorFactory BehaviorFactory { get; }
        ICheckFactory CheckFactory { get; }
        INZazuDataSerializer Serializer { get; }

        INZazuWpfView View { get; set; }
    }
}