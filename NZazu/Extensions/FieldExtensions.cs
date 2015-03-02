using System;
using System.Windows.Controls.Primitives;

namespace NZazu.Extensions
{
    public static class FieldExtensions
    {
        public static bool IsValid(this INZazuWpfField field)
        {
            if (field == null) throw new ArgumentNullException("field");
            return field.Validate().IsValid;
        }

        public static bool IsReadOnly(this INZazuWpfField field)
        {
            if (field == null) throw new ArgumentNullException("field");
            var control = field.ValueControl;
            if (control == null) return true;

            var textBox = control as TextBoxBase;
            return textBox != null ? textBox.IsReadOnly : !control.IsEnabled;
        }

        public static void SetReadOnly(this INZazuWpfField field, bool isReadOnly)
        {
            if (field == null) throw new ArgumentNullException("field");
            var control = field.ValueControl;
            if (control == null) return;

            var textBox = control as TextBoxBase;
            if (textBox != null)
                textBox.IsReadOnly = isReadOnly;
            else
                control.IsEnabled = !isReadOnly;
        }
    }
}