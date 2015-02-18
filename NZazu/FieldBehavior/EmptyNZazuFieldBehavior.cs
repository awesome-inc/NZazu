using System;

namespace NZazu.FieldBehavior
{
    internal class EmptyNZazuFieldBehavior : NZazuFieldBehavior
    {
        public override void AttachTo(INZazuWpfField field, INZazuWpfView view)
        {
            if (field == null) throw new ArgumentNullException("field");
            if (view == null) throw new ArgumentNullException("view");
        }

        public override void Detach() { }
    }
}