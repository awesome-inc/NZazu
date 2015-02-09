using System.Windows.Controls;

namespace NZazu
{
    public interface INZazuWpfFieldBehavior
    {
        void AttachTo(Control valueControl);
        void Detach();
    }
}