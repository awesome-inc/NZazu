using System;
using System.Windows.Controls;

namespace NZazu.FieldBehavior
{
    internal class EmptyNZazuFieldBehavior : NZazuFieldBehavior
    {
        public override void AttachTo(Control valueControl)
        {
            if (valueControl == null) throw new ArgumentNullException("valueControl");
        }
        public override void Detach() { }
    }
}