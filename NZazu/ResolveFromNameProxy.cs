using System;

namespace NZazu
{
    public class ResolveFromNameProxy<T> : IResolveFromName<T>
    {
        private readonly Func<string, T> _factory;

        public ResolveFromNameProxy(Func<string, T> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            _factory = factory;
        }

        public T Resolve(string name)
        {
            return _factory(name);
        }
    }
}