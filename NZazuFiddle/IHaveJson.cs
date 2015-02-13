namespace NZazuFiddle
{
    public interface IHaveJsonFor<T>
    {
        T Model { get; set; }
        string Json { get; set; }
        bool HasJsonError { get; }
        string JsonError { get; }
    }
}