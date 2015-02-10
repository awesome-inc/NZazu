using System;

namespace NZazu
{
    public class ResolveFromNamesProxy<T> : IResolveFromNames<T>
    {
        private readonly Func<string, T> _factory;

        public ResolveFromNamesProxy(Func<string,T> factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            _factory = factory;
        }

        public T Resolve(string name)
        {
            return _factory(name);
        }
    }
}