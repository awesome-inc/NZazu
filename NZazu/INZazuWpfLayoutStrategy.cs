using System.Collections.Generic;
using System.Windows.Controls;

namespace NZazu
{
    public interface INZazuWpfLayoutStrategy
    {
        void DoLayout(ContentControl container, IEnumerable<INZazuWpfField> fields);
    }
}