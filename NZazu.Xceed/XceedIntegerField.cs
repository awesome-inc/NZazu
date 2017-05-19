using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using NZazu.Contracts;
using NZazu.Fields;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    public class XceedIntegerField : NZazuIntegerField
    {
        public XceedIntegerField(FieldDefinition definition) : base(definition) { }

        public override DependencyProperty ContentProperty => IntegerUpDown.ValueProperty;

        protected override Control CreateValueControl()
        {
            var control = new IntegerUpDown {ToolTip = Description, Watermark = Hint};
            var formatString = GetSetting("Format");
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
            if (string.IsNullOrWhiteSpace(formatString)) return false;
            return SpecifierPattern.IsMatch(formatString);
        }
    }
}