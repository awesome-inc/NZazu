using System.ComponentModel;
using NZazu.Contracts;

namespace NZazuFiddle
{
    public interface IFormDataViewModel : INotifyPropertyChanged
    {
        FormData FormData { get; set; }
    }
}