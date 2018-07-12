using System.Collections.Generic;
using NZazu.Contracts;
using NZazu.Contracts.Checks;

namespace NZazu
{
    public interface INZazuWpfView : INZazuView
    {
        INZazuWpfFieldFactory FieldFactory { get; set; }
        IResolveLayout ResolveLayout { get; set; }
        INZazuWpfField GetField(string key);
        bool TryGetField(string key, out INZazuWpfField field);
    }
}