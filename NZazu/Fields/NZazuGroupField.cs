using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuGroupField
        : NZazuField
        , INZazuWpfFieldContainer
    {
        public NZazuGroupField(FieldDefinition definition) : base(definition) { }

        public override bool IsEditable => false;
        private string _stringValue;
        private INZazuWpfFieldFactory _factory;
        public override DependencyProperty ContentProperty => null;
        public override string Type => "group";
        public string Layout { get; set; }

        protected override void SetStringValue(string value) { _stringValue = value; }
        protected override string GetStringValue() { return _stringValue; }
        public IEnumerable<INZazuWpfField> Fields { get; set; }

        protected override Control CreateValueControl()
        {
            if (Definition == null) throw new ArgumentNullException(nameof(Definition));

            Control control = string.IsNullOrEmpty(Description) 
                ? new ContentControl { Focusable = false } 
                : new GroupBox { Focusable = false, Header = Description };

            return control;
        }

        public override NZazuField Initialize(Func<Type, object> propertyLookup)
        {
            _factory = (INZazuWpfFieldFactory)propertyLookup(typeof(INZazuWpfFieldFactory));

            CreateChildControls();

            return base.Initialize(propertyLookup);
        }

        private void CreateChildControls()
        {
            if (!string.IsNullOrWhiteSpace(Definition.Layout))
                Layout = Definition.Layout;

            if (Definition.Fields == null || !Definition.Fields.Any()) return;

            Fields = Definition.Fields.Select(_factory.CreateField).ToArray();
        }
    }
}