using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazu
{
    public interface INZazuWpfFieldContainer : INZazuWpfField
    {
        void CreateChildControls(INZazuWpfFieldFactory factory, FieldDefinition containerDefinition);
        IEnumerable<INZazuWpfField> Fields { get; set; }
        string Layout { get; set; }
    }
}