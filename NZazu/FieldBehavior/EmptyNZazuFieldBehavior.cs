using System;

namespace NZazu.FieldBehavior
{
    internal class EmptyNZazuFieldBehavior : NZazuFieldBehavior
    {
        public override void AttachTo(INZazuWpfField field)
        {
            if (field == null) throw new ArgumentNullException("field");
        }

        public override void Detach() { }
    }
}