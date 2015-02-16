namespace NZazu.FieldBehavior
{
    internal abstract class NZazuFieldBehavior : INZazuWpfFieldBehavior
    {
        public abstract void AttachTo(INZazuWpfField field);
        public abstract void Detach();
    }
}