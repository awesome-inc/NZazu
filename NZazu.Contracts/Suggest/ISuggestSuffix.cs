namespace NZazu.Contracts.Suggest
{
    public interface ISuggestSuffix
    {
        string For(string prefix, string firstMatch);
    }
}