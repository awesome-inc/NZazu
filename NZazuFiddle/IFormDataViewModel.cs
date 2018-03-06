using Caliburn.Micro;
using NZazu.Contracts;

namespace NZazuFiddle
{
    public interface IFormDataViewModel : IScreen, IHaveFormData, IHaveJsonFor<FormData>
    {
    }
}