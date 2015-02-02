using System.Collections.Generic;
using System.Windows.Controls;

namespace NZazu
{
    public interface INZazuLayoutStrategy
    {
        void DoLayout(ContentControl container, IEnumerable<INZazuField> fields);
    }
}