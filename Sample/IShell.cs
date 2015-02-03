using System.Collections.Generic;

namespace Sample
{
    public interface IShell
    {
        IEnumerable<INZazuSample> Samples { get; set; }
        INZazuSample SelectedSample { get; set; }
    }
}