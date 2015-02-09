using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using NZazu.FieldFactory;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    public class XceedIntegerField : NZazuIntegerField
    {
        public XceedIntegerField(string key) : base(key)
        {
        }

        public override DependencyProperty ContentProperty
        {
            get { return IntegerUpDown.ValueProperty; }
        }

        protected override Control GetValue()
        {
            var control = new IntegerUpDown {ToolTip = Description, Watermark = Hint};
            var formatString = GetFormatString();
            if (IsSupported(formatString))
            {
                control.FormatString = formatString;
            }
            return control;
        }

        // cf.: http://wpftoolkit.codeplex.com/wikipage?title=IntegerUpDown&referringTitle=NumericUpDown-derived%20controls#formatstring
        private static readonly Regex SpecifierPattern = new Regex(@"[C,F,G,N,P]{1}\d*", RegexOptions.Compiled);
        private static bool IsSupported(string formatString)
        {
            if (String.IsNullOrWhiteSpace(formatString)) return false;
            return SpecifierPattern.IsMatch(formatString);
        }
    }
}