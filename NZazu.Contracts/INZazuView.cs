namespace NZazu.Contracts
{
    public interface INZazuView
    {
        FormDefinition FormDefinition { get; set; }
        INZazuField GetField(string fieldKey);
    }
}