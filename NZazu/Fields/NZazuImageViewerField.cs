using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NZazu.Contracts;
using NZazu.Contracts.Checks;

namespace NZazu.Fields
{
    public class NZazuImageViewerField : NZazuField
    {
        private readonly IEnumerable<string> _values;
        private Control _clientControl;
        private string _customValue;
        private string _stringValue;

        public NZazuImageViewerField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
            _values = Definition.Values ?? Enumerable.Empty<string>();
        }

        public override DependencyProperty ContentProperty => Image.SourceProperty;

        public override void SetValue(string value)
        {
            var allowCustomValues = Definition.Settings.Get<bool>("AllowCustomValues");
            _stringValue = value;

            // lets see if the value is a URI if not set the stringValue to null
            // ReSharper disable once UnusedVariable
            if (!Uri.TryCreate(value, UriKind.Absolute, out var uriOut))
                _stringValue = null;

            // ok, lets see if the value is not part of the options list
            if (value != null && !_values.Contains(value))
                if (allowCustomValues != null && allowCustomValues.Value)
                {
                    _customValue = value;
                }
                else
                {
                    _customValue = null;
                    _stringValue = null;
                }

            // in case we allow custom values and the value is not part of the value list
            //if (value != null && allowCustomValues != null && allowCustomValues.Value && !Definition.Values.Contains(value))
            //    _customValue = value;

            // set the image to the uri
            var image = ((_clientControl as ContentControl)?.Content as Border)?.Child as Image;
            if (image == null) return;
            image.Source = string.IsNullOrEmpty(_stringValue)
                ? BitmapSource.Create(2, 2, 96, 96, PixelFormats.Indexed1,
                    new BitmapPalette(new List<Color> {Colors.Transparent}), new byte[] {0, 0, 0, 0}, 1)
                // ReSharper disable once AssignNullToNotNullAttribute
                : new BitmapImage(new Uri(value));
        }

        public override string GetValue()
        {
            return _stringValue;
        }

        public override ValueCheckResult Validate()
        {
            return ValueCheckResult.Success;
        }

        protected override Control CreateValueControl()
        {
            if (_clientControl != null) return _clientControl;

            _clientControl = new ContentControl
            {
                Content = new Border
                {
                    BorderBrush = Brushes.Silver,
                    BorderThickness = new Thickness(1),
                    Child = new Image
                    {
                        ToolTip = Definition.Description,
                        Focusable = true
                    }
                }
            };
            _clientControl.PreviewKeyDown += ClientControl_PreviewKeyDown;
            _clientControl.PreviewMouseWheel += ClientControl_MouseWheel;
            _clientControl.PreviewMouseLeftButtonUp += ClientControl_MouseLeftButtonUp;

            SetValue(_stringValue);

            return _clientControl;
        }

        [ExcludeFromCodeCoverage]
        private void ClientControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ToggleValues(e.Delta < 0);
        }

        [ExcludeFromCodeCoverage]
        private void ClientControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as Control)?.Focus();

            ToggleValues();
        }

        [ExcludeFromCodeCoverage]
        private void ClientControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Space) return;

            ToggleValues();
        }

        internal void ToggleValues(bool toggleBack = false)
        {
            var allowNullValues = Definition.Settings.Get<bool>("AllowNullValues");
            var allowCustomValues = Definition.Settings.Get<bool>("AllowCustomValues");

            var options = _values.ToArray();
            if (allowCustomValues != null && allowCustomValues.Value && !string.IsNullOrEmpty(_customValue))
                options = options.Concat(new[] {_customValue}).ToArray();

            var currentValueIsAt = -1;
            for (var i = 0; i < options.Length; i++)
                if (string.Compare(options[i], GetValue(), StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    currentValueIsAt = i;
                    break;
                }

            // just in case no options are given (and no custom value!)
            if (options.Length == 0)
            {
                SetValue(null);
                return;
            }

            if (toggleBack)
                if (allowNullValues != null && allowNullValues.Value && currentValueIsAt == 0)
                    SetValue(null);
                else
                    SetValue(options[(Math.Max(currentValueIsAt, 0) + options.Length - 1) % options.Length]);
            else if (allowNullValues != null && allowNullValues.Value && currentValueIsAt == options.Length - 1)
                SetValue(null);
            else
                SetValue(options[(currentValueIsAt + 1) % options.Length]);
        }
    }
}