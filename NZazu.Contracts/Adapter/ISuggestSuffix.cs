namespace NZazu.Contracts.Adapter
{
    public interface ISuggestSuffix
    {
        string For(string prefix, string firstMatch);
    }
}