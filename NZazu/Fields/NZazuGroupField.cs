using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuGroupField : NZazuField, INZazuWpfFieldContainer
    {
        public NZazuGroupField(FieldDefinition definition) : base(definition) { }

        public override bool IsEditable => false;
        public override string StringValue { get; set; }
        public override DependencyProperty ContentProperty => null;
        public override string Type => "group";
        public string Layout { get; set; }
        public IEnumerable<INZazuWpfField> Fields { get; set; }

        protected override Control GetValue()
        {
            if (string.IsNullOrEmpty(Description))
                return new ContentControl { Focusable = false };
            else
                return new GroupBox { Focusable = false, Header = Description };
        }

        public void CreateChildControls(INZazuWpfFieldFactory factory, FieldDefinition containerDefinition)
        {
            if (containerDefinition == null) throw new ArgumentNullException(nameof(containerDefinition));

            if (!string.IsNullOrWhiteSpace(containerDefinition.Layout))
                this.Layout = containerDefinition.Layout;

            if (containerDefinition.Fields == null || !containerDefinition.Fields.Any()) return;

            this.Fields = containerDefinition.Fields.Select(factory.CreateField).ToArray();
        }
    }
}