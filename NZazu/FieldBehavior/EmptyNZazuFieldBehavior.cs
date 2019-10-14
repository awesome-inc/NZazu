using System;

namespace NZazu.FieldBehavior
{
    internal class EmptyNZazuFieldBehavior : NZazuFieldBehavior
    {
        public override void AttachTo(INZazuWpfField field, INZazuWpfView view)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
        }

        public override void Detach()
        {
        }
    }
}