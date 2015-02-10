using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NZazu.Fields
{
    public class NZazuOptionsField : NZazuField<string>
    {
        public NZazuOptionsField(string key) : base(key)
        {
        }

        public override DependencyProperty ContentProperty
        {
            get { return Selector.SelectedItemProperty; }
        }

        public override string Type { get { return "option"; } }

        public string[] Options { get; protected internal set; }

        protected override Control GetValue()
        {
            var control = new ComboBox { ToolTip = Description};
            if (Options != null)
            {
                foreach (var option in Options)
                    control.Items.Add(option);
                control.SelectedItem = Options.FirstOrDefault();
            }
            return control;
        }

        protected override void SetStringValue(string value)
        {
            throw new System.NotImplementedException();
        }

        protected override string GetStringValue()
        {
            throw new System.NotImplementedException();
        }
    }
}