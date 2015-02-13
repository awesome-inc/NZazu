using Caliburn.Micro;
using NZazu.Contracts;

namespace NZazuFiddle
{
    public interface IHaveFormDefinition : IHandle<FormDefinition>
    {
        FormDefinition Definition { get; set; }
    }
}