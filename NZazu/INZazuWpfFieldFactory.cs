using NZazu.Contracts;

namespace NZazu
{
    public interface INZazuWpfFieldFactory
    {
        INZazuWpfField CreateField(FieldDefinition fieldDefinition, int rowIdx = -1);

        INZazuWpfFieldBehaviorFactory BehaviorFactory { get; }
        ICheckFactory CheckFactory { get; }
        INZazuTableDataSerializer Serializer { get; }

        INZazuWpfView View { get; set; }
    }
}