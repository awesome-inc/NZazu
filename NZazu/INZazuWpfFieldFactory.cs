using NZazu.Contracts;

namespace NZazu
{
    public interface INZazuWpfFieldFactory
    {
        INZazuWpfField CreateField(FieldDefinition fieldDefinition);

        INZazuWpfFieldBehaviorFactory BehaviorFactory { get; }
        ICheckFactory CheckFactory { get; }
        INZazuTableDataSerializer Serializer { get; }

        INZazuWpfView View { get; set; }
    }
}