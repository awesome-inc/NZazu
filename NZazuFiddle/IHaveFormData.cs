using Caliburn.Micro;
using NZazu.Contracts;

namespace NZazuFiddle
{
    public interface IHaveFormData : IHandle<FormData>
    {
        FormData Data { get; set; }
    }
}