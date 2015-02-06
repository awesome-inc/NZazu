using NZazu.Contracts.Checks;

namespace NZazu.Contracts
{
    public interface ICheckFactory
    {
        IValueCheck CreateCheck(CheckDefinition checkDefinition);
    }
}