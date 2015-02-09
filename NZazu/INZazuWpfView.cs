using NZazu.Contracts;
using NZazu.Contracts.Checks;

namespace NZazu
{
    public interface INZazuWpfView
    {
        FormDefinition FormDefinition { get; set; }
        FormData FormData { get; set; }
            
        INZazuWpfFieldFactory FieldFactory { get; set; }
        IResolveLayout ResolveLayout { get; set; }

        INZazuWpfField GetField(string key);
        void ApplyChanges();

        /// <exception cref="ValidationException"></exception>
        void Validate();
    }
}