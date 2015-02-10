using System.Windows;
using System.Windows.Controls;

namespace NZazu.Fields
{
    class NZazuLabelField : NZazuField
    {
        public NZazuLabelField(string key) : base(key)
        {
        }

        public override string Type { get { return "label"; } }

        public override string StringValue 
        {
            get { return null; }
            set {} }

        public override DependencyProperty ContentProperty
        {
            get { return null; }
        }

        protected override Control GetValue() { return !string.IsNullOrWhiteSpace(Description) ? new Label { Content = Description } : null; }
    }
}