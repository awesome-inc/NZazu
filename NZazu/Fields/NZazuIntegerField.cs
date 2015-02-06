using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace NZazu.Fields
{
    class NZazuIntegerField : NZazuField<int?>
    {
        public NZazuIntegerField(string key) : base(key) { }

        public override string Type { get { return "int"; } }
        protected internal override DependencyProperty ContentProperty { get { return TextBox.TextProperty; } }

        protected override Control GetValue()
        {
            return new TextBox { ToolTip = Description };
        }

        protected override void SetStringValue(string value)
        {
            int i;
            if (int.TryParse(value, out i)) Value = i;
            else Value = null;
        }

        protected override string GetStringValue()
        {
            return Value.HasValue ? Value.Value.ToString(CultureInfo.InvariantCulture) : String.Empty;
        }
    }
    class NZazuDateField : NZazuField<DateTime?>
    {
        public NZazuDateField(string key) : base(key) { }

        public override string Type { get { return "date"; } }
        protected internal override DependencyProperty ContentProperty { get { return DatePicker.SelectedDateProperty; } }

        protected override Control GetValue()
        {
            return new DatePicker { ToolTip = Description };
        }

        protected override void SetStringValue(string value)
        {
            DateTime dt;
            if (DateTime.TryParse(value, out dt)) Value = dt;
            else Value = null;
        }
        // todo culture
        protected override string GetStringValue()
        {
            return Value.HasValue ? Value.Value.ToString(CultureInfo.InvariantCulture) : String.Empty;
        }
    }
}

