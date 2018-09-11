using System;
using NZazu.Contracts;

namespace NZazu
{
    public interface INZazuWpfFieldFactory
    {
        INZazuWpfField CreateField(FieldDefinition fieldDefinition, int rowIdx = -1);

        T Resolve<T>(Type x = null);
        void Use<T>(T service);
    }
}