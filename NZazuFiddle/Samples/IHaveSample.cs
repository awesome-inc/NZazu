namespace NZazuFiddle.Samples
{
    public interface IHaveSample
    {
        ISample Sample { get; }
        int Order { get; }
    }
}