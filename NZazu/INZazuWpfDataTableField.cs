using System.Collections.Generic;

namespace NZazu
{
    public interface INZazuWpfDataTableField : INZazuWpfField
    {
        IEnumerable<INZazuWpfField> Fields { get; }
        string Layout { get; }
    }
}