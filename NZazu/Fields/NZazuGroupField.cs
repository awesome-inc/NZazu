using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;
using NZazu.Contracts.Checks;

namespace NZazu.Fields
{
    public class NZazuGroupField
        : NZazuField
            , INZazuWpfFieldContainer
    {
        private readonly INZazuWpfFieldFactory _factory;

        public NZazuGroupField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
            _factory = (INZazuWpfFieldFactory) serviceLocatorFunc(typeof(INZazuWpfFieldFactory));

            IsEditable = false;
            CreateChildControls();
        }

        public override DependencyProperty ContentProperty => null;
        public string Layout { get; set; }

        public override void SetValue(string value)
        {
        }

        public override string GetValue()
        {
            return null;
        }

        public override ValueCheckResult Validate()
        {
            return ValueCheckResult.Success;
        }

        public IEnumerable<INZazuWpfField> Fields { get; set; }

        protected override Control CreateValueControl()
        {
            if (Definition == null) throw new ArgumentNullException(nameof(Definition));

            Control control = string.IsNullOrEmpty(Definition.Description)
                ? new ContentControl {Focusable = false}
                : new GroupBox {Focusable = false, Header = Definition.Description};

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