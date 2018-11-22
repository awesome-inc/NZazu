using NEdifis.Attributes;

namespace NZazu.EventArgs
{
    [ExcludeFromConventions("this is just an event arg class")]
    public class FieldFocusChangedEventArgs
    {
        public FieldFocusChangedEventArgs(INZazuWpfField oldFocusedElement, INZazuWpfField newFocusedElement)
        {
            OldFocusedElement = oldFocusedElement;
            NewFocusedElement = newFocusedElement;
        }
        public INZazuWpfField OldFocusedElement { get;  }
        public INZazuWpfField NewFocusedElement { get;  }
    }
}