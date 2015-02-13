using NZazu.Contracts;

namespace NZazuFiddle
{
    public interface IFormDataViewModel : IHaveFormData, IHaveJsonFor<FormData>
    {
    }
}