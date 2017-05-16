using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

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