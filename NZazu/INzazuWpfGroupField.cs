using System.Collections.Generic;

namespace NZazu
{
    public interface INZazuWpfGroupField
    {
        IEnumerable<INZazuWpfField> Fields { get; } 
    }
}