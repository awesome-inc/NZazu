using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NZazu.Fields
{
    public class NZazuGroupField : NZazuField, INZazuWpfGroupField
    {
        public NZazuGroupField(string key) : base(key)
        {
            Fields = Enumerable.Empty<INZazuWpfField>();
        }

        public override bool IsEditable => false;
        public override string StringValue { get; set; }

        public override DependencyProperty ContentProperty => null;
        public override string Type => "group";

        protected override Control GetValue()
        {
            return new ContentControl {Focusable = false};
        }

        public IEnumerable<INZazuWpfField> Fields { get; protected internal set; }
        public string Layout { get; protected internal set; }
    }
}