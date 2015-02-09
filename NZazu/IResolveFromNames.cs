namespace NZazu
{
    public interface IResolveFromNames<out T>
    {
        T Resolve(string name);
    }
}