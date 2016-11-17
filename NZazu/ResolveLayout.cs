using System.Collections.Generic;
using NZazu.LayoutStrategy;

namespace NZazu
{
    public class ResolveLayout : IResolveLayout
    {
        private static readonly INZazuWpfLayoutStrategy Default = new GridLayout();

        private readonly Dictionary<string, INZazuWpfLayoutStrategy> _layouts =
            new Dictionary<string, INZazuWpfLayoutStrategy>
            {
                {"stack", new StackedLayout()}
            };

        public INZazuWpfLayoutStrategy Resolve(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return Default;
            INZazuWpfLayoutStrategy layout;
            return _layouts.TryGetValue(name, out layout) ? layout : Default;
        }
    }
}