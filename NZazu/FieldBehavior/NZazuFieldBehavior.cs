namespace NZazu.FieldBehavior
{
    internal abstract class NZazuFieldBehavior : INZazuWpfFieldBehavior
    {
        public abstract void AttachTo(INZazuWpfField field, INZazuWpfView view);
        public abstract void Detach();
    }
}