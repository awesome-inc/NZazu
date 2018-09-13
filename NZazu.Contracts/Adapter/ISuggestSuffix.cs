namespace NSuggest
{
    public interface ISuggestSuffix
    {
        string For(string prefix, string firstMatch);
    }
}