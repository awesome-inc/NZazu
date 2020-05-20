using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NZazu.Contracts;
using NZazu.Fields;
using Xceed.Wpf.Toolkit;
using RichTextBox = Xceed.Wpf.Toolkit.RichTextBox;

namespace NZazu.Xceed
{
    public class XceedRichTextField : NZazuTextField
    {
        public const double DefaultHeight = 80.0d;

        public XceedRichTextField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
            ValueConverter = new RtfSpecialCharactersConverter();
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
                TextFormatter = GetFormatter(Definition.Settings.Get("Format"))
            };

            var showFormatBar = Definition.Settings.Get<bool>("ShowFormatBar");
            if (showFormatBar == true)
                RichTextBoxFormatBarManager.SetFormatBar(control, new RichTextBoxFormatBar());

            return control;
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

        private static string FromPlainToRtf(string input)
        {
            var unicodeCharacterRegex = new Regex(@"[\p{L}-[A-Za-z]]");
            var convertedValues = new List<char>();

            foreach (Match match in unicodeCharacterRegex.Matches(input))
            {
                if (!match.Success) continue;

                // we can safely convert to char here, because this regex only catches one char at a time
                var matchValue = match.Value.FirstOrDefault();
                if (convertedValues.Contains(matchValue)) continue; 

                var converted = $"\\'{System.Convert.ToInt32(matchValue):X}".ToLower();

                input = input.Replace(matchValue.ToString(), converted);
                convertedValues.Add(matchValue);
            }

            return input;
        }

        private static string FromRtfToPlain(string input)
        {
            var rtfSpecialCharactersRegex = new Regex(@"(?<specialChar>\\{1,2}'[1-9,a-f]{2})");
            var convertedValues = new List<string>();

            foreach (Match match in rtfSpecialCharactersRegex.Matches(input))
            {
                if (!match.Success) continue;

                var matchValue = match.Groups["specialChar"].Value;
                if (convertedValues.Contains(matchValue)) continue;

                // get only the part behind the ' , this indicates the hex-value for the char
                var hexValue = matchValue.Substring(matchValue.IndexOf("'", StringComparison.InvariantCulture) + 1);
                var @char = (char) short.Parse(hexValue, NumberStyles.AllowHexSpecifier);

                // adjust the input string
                input = input.Replace(matchValue, @char.ToString());
                convertedValues.Add(matchValue);
            }

            return input;
        }
    }

    #endregion

}