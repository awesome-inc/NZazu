using System;
using System.Windows.Markup;
using NEdifis.Attributes;

// cf: https://social.msdn.microsoft.com/Forums/vstudio/en-US/549eeb7c-8df7-4a6c-a264-91f06ca75293/debug-wpf-binding?forum=wpf
namespace NZazu.Extensions
{
    [ExcludeFromConventions("stolen from StackOverflow")]
    public class DebugExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return NoExceptionsConverter.Instance;
        }
    }
}