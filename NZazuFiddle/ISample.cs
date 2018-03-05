using NZazuFiddle.TemplateManagement.Contracts;

namespace NZazuFiddle
{
    public interface ISample
    {
        ETemplateStatus Status { get; set; }
        string Name { get; }
        string Description { get; }
        string Id { get; }

        IFiddle Fiddle { get; }
    }
}