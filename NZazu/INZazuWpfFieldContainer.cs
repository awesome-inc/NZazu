using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazu
{
    public interface INZazuWpfFieldContainer : INZazuWpfField
    {
        IEnumerable<INZazuWpfField> Fields { get; set; }
        string Layout { get; set; }
    }
}