using Caliburn.Micro;

namespace NZazuFiddle
{
    public interface IEndpointViewModel : IScreen
    {
        string Endpoint { get; set; }
        bool CanLoadSamples { get; }
        bool CanSaveSamples { get; }
        void LoadSamples();
        void SaveSamples();
    }
}