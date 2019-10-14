using System.Collections.Generic;

namespace NZazu
{
    public interface INZazuWpfFieldContainer : INZazuWpfField
    {
        IEnumerable<INZazuWpfField> Fields { get; set; }
        string Layout { get; set; }
    }
}