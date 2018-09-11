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
        public NZazuGroupField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc) {
            _factory = (INZazuWpfFieldFactory)serviceLocatorFunc(typeof(INZazuWpfFieldFactory));

            IsEditable = false;
            CreateChildControls();
}

        private readonly INZazuWpfFieldFactory _factory;
        public override DependencyProperty ContentProperty => null;
        public string Layout { get; set; }

        public override void SetStringValue(string value) {  }
        public override string GetStringValue(){return null; }

        public IEnumerable<INZazuWpfField> Fields { get; set; }

        protected override Control CreateValueControl()
        {
            if (Definition == null) throw new ArgumentNullException(nameof(Definition));

            Control control = string.IsNullOrEmpty(Definition.Description) 
                ? new ContentControl { Focusable = false } 
                : new GroupBox { Focusable = false, Header = Definition.Description };

            return control;
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