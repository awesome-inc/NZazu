using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using NZazu.Contracts;
using NZazu.Fields;
using Xceed.Wpf.Toolkit;
using RichTextBox = Xceed.Wpf.Toolkit.RichTextBox;

namespace NZazu.Xceed
{
    public class XceedRichTextField : NZazuTextField
    {
        public const double DefaultHeight = 80.0d;
        private readonly ITextFormatter _formatter;

        public XceedRichTextField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
            _formatter = GetFormatter(Definition.Settings.Get("Format"));
        }

        public override DependencyProperty ContentProperty => RichTextBox.TextProperty;

        protected override Control CreateValueControl()
        {
            var control = new RichTextBox
            {
                ToolTip = Definition.Description,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                MinHeight = DefaultHeight,
                MaxHeight = DefaultHeight,
                TextFormatter = _formatter
            };

            var showFormatBar = Definition.Settings.Get<bool>("ShowFormatBar");
            if (showFormatBar == true)
                RichTextBoxFormatBarManager.SetFormatBar(control, new RichTextBoxFormatBar());

            return control;
        }

        protected override Binding DecorateBinding(Binding binding)
        {
            var decorated = base.DecorateBinding(binding);
            decorated.Converter = new RtfSpecialCharactersConverter(_formatter);
            return decorated;
        }

        private static ITextFormatter GetFormatter(string format)
        {
            switch (format)
            {
                case "plain": return new PlainTextFormatter();
                case "xaml": return new XamlFormatter();
                // ReSharper disable once RedundantCaseLabel
                case "rtf":
                default: return new RtfFormatter();
            }
        }
    }

    #region converter

    internal class RtfSpecialCharactersConverter : IValueConverter
    {
        private readonly RichTextBox _richTextBox;

        public RtfSpecialCharactersConverter(ITextFormatter formatter = null)
        {
            _richTextBox = new RichTextBox
            {
                TextFormatter = formatter ?? new RtfFormatter()
            };
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string valueString)) return Binding.DoNothing;
            if (targetType != typeof(string)) return Binding.DoNothing;

            return FromPlainToRtf(valueString);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string valueString)) return Binding.DoNothing;
            if (targetType != typeof(string)) return Binding.DoNothing;

            return FromRtfToPlain(valueString);
        }

        // HACK:
        // using an instance of a RichTextBox here to convert from and to RTF
        // this is done, because the conversion is rather complex
        // and I don't wanna do all of this by hand
        // additionally there are different ITextFormatters that are also to be considered
        // so the easiest way to do this is to use the RichTextBox itself
        // because it knows it's conversion-stuff best!

        private string FromPlainToRtf(string input)
        {
            var result = string.Empty;
            _richTextBox.Dispatcher.Invoke(() =>
            {
                _richTextBox.Clear();
                _richTextBox.AppendText(input);

                result = _richTextBox.Text;
            });
            
            return result;
        }

        private string FromRtfToPlain(string input)
        {
            var result = string.Empty;
            _richTextBox.Dispatcher.Invoke(() =>
            {
                _richTextBox.Clear();
                _richTextBox.Text = input;

                var textRange = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
                result = textRange.Text.Trim(' ', '\r', '\n', '\t', '}', '{');
            });

            return result;
        }
    }

    #endregion

}