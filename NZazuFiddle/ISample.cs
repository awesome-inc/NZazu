using NZazuFiddle.TemplateManagement.Contracts;
using System.Windows.Media;

namespace NZazuFiddle
{
    public interface ISample
    {
        ETemplateStatus Status { get; set; }
        Brush StatusBrush { get; }
        string Name { get; }
        string Description { get; }
        string Id { get; }

        IFiddle Fiddle { get; }
    }
}