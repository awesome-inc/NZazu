namespace NZazu.Contracts
{
    public interface INZazuField
    {
        string Key { get; }
        string Type { get; }
        string Prompt { get; }
        string Description { get; }
    }
}