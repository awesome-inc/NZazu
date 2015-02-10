using System.Windows.Controls;

namespace NZazu.FieldBehavior
{
    internal abstract class NZazuFieldBehavior : INZazuWpfFieldBehavior
    {
        public abstract void AttachTo(Control valueControl);
        public abstract void Detach();
    }
}