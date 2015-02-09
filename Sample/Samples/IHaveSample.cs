namespace Sample.Samples
{
    public interface IHaveSample
    {
        INZazuSample Sample { get; }
        int Order { get; }
    }
}