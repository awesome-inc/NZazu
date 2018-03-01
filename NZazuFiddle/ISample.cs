namespace NZazuFiddle
{
    public interface ISample
    {
        string Name { get; }
        string Description { get; }
        string Id { get; }

        IFiddle Fiddle { get; }
    }
}