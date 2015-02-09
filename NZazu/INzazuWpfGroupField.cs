using System.Collections.Generic;

namespace NZazu
{
    public interface INZazuWpfGroupField : INZazuWpfField
    {
        IEnumerable<INZazuWpfField> Fields { get; } 
    }
}