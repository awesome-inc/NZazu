using System.Collections.Generic;

namespace NZazuFiddle
{
    public interface IShell
    {
        IEnumerable<ISample> Samples { get; set; }
        ISample SelectedSample { get; set; }
    }
}