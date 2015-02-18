namespace NZazu
{
    public interface INZazuWpfFieldBehavior
    {
        void AttachTo(INZazuWpfField field, INZazuWpfView view);
        void Detach();
    }
}