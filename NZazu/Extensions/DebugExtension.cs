using System;
using System.Windows.Markup;

public class DebugExtension : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return DebugConverter.Instance;
    }
}