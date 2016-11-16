using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;
using NZazu.Fields.Controls;

namespace NZazu.Fields
{
    public class NZazuDataTableField
        : NZazuField
    {
        private readonly DynamicDataTable _clientControl;

        public NZazuDataTableField(string key, FieldDefinition definition) : base(key, definition)
        {
            _clientControl = new DynamicDataTable();
        }

        public override bool IsEditable => false;
        public override string StringValue { get; set; }
        public override DependencyProperty ContentProperty => DynamicDataTable.ValuesAsJsonProperty;
        public override string Type => "datatable";

        protected override Control GetValue()
        {
            return _clientControl;
        }
    }
}