using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using NZazu.Contracts;

namespace NZazu.Fields
{
    public class NZazuImageViewerField : NZazuField
    {
        private Control _clientControl;
        private string _stringValue;
        public NZazuImageViewerField(FieldDefinition definition) : base(definition) { }


        public override string Type => "imageViewer";
        public override bool IsEditable => true;

        protected override void SetStringValue(string value)
        {
            _stringValue = value;

            if ((Image)((ContentControl)_clientControl)?.Content == null) return;
            ((Image)((ContentControl)_clientControl).Content).Source = string.IsNullOrEmpty(_stringValue)
                ? null
                : new BitmapImage(new Uri(value));
        }

        protected override string GetStringValue()
        {
            return _stringValue;
        }

        public override DependencyProperty ContentProperty => ToggleButton.IsCheckedProperty;

        protected override Control GetValue()
        {
            if (_clientControl != null) return _clientControl;

            _clientControl = new ContentControl
            {
                Content = new Image
                {
                    ToolTip = Description,
                }
            };
            _clientControl.PreviewKeyDown += ClientControl_PreviewKeyDown;

            SetStringValue(_stringValue);

            return _clientControl;
        }

        private void ClientControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Space) return;

            var options = Definition.Values;
            var currentValueIsAt = -1;
            for (var i = 0; i < options.Length - 1; i++)
                if (string.Compare(options[i], GetStringValue(), StringComparison.CurrentCultureIgnoreCase) == 0)
                    { currentValueIsAt = i; break; }

            var newValue = (currentValueIsAt + 1) % (options.Length - 1);

            this.StringValue = options[newValue];
        }
    }
}