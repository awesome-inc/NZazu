using NZazu.Contracts;

namespace NZazuFiddle
{
    public interface IFormDefinitionViewModel
        : IHaveFormDefinition
            , IHaveJsonFor<FormDefinition>
    {
    }
}