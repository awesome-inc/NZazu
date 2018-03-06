using System.Collections.Generic;

namespace NZazuFiddle
{
    public interface IShell
    {
        IEndpointViewModel EndpointViewModel { get; }

        IEnumerable<ISample> Samples { get; set; }
        ISample SelectedSample { get; set; }
    }
}