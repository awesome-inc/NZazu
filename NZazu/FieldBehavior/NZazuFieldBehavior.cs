using NEdifis.Attributes;

namespace NZazu.FieldBehavior
{
    [ExcludeFromConventions("internal and pure abstract")]
    internal abstract class NZazuFieldBehavior : INZazuWpfFieldBehavior
    {
        public abstract void AttachTo(INZazuWpfField field, INZazuWpfView view);
        public abstract void Detach();
    }
}