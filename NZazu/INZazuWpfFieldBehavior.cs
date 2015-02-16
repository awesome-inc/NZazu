namespace NZazu
{
    public interface INZazuWpfFieldBehavior
    {
        void AttachTo(INZazuWpfField field);
        void Detach();
    }
}