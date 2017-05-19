using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using NEdifis.Attributes;

// cf: https://social.msdn.microsoft.com/Forums/vstudio/en-US/549eeb7c-8df7-4a6c-a264-91f06ca75293/debug-wpf-binding?forum=wpf
namespace NZazu.Extensions
{
    [ExcludeFromConventions("stolen from StackOverflow")]
    public class DebugConverter : IValueConverter
    {
        public static DebugConverter Instance = new DebugConverter();
        internal DebugConverter() { }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value; //Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value; //Binding.DoNothing;
        }

        #endregion
    }
}