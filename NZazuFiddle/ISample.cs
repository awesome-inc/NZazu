namespace NZazuFiddle
{
    public interface ISample
    {
        string Name { get; }
        string Description { get; }

        IFiddle Fiddle { get; }
    }
}