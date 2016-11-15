using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuDataTableField
        : NZazuField
        , INZazuWpfControlContainer
    {
        public NZazuDataTableField(string key) : base(key)
        {
            Fields = Enumerable.Empty<INZazuWpfField>();
        }

        public override bool IsEditable => false;
        public override string StringValue { get; set; }

        public override DependencyProperty ContentProperty => null;
        public override string Type => "datatable";

        protected override Control GetValue()
        {
            return new ContentControl { Focusable = false };
        }

        public void CreateChildControls(INZazuWpfFieldFactory factory, FieldDefinition containerDefinition)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<INZazuWpfField> Fields { get; set; }
        public string Layout { get; set; }
    }
}