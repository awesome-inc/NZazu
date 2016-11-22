namespace NZazu.Serializer
{
    public interface IRequireSerializer
    {
        INZazuDataSerializer Serializer { get; set; }
    }
}